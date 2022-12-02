import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import './App.css'
import { Websocket, WebsocketBuilder } from 'websocket-ts';
import IChampSelectState from './data/IChampSelectState';


class AppInit {
  socket?: Websocket;
  backend?: string;

  getQueryVariable = function (variable: string) {
    const query = window.location.search.substring(1);
    const vars = query.split('&');
    for (let i = 0; i < vars.length; i++) {
      const pair = vars[i].split('=');
      if (decodeURIComponent(pair[0]) === variable) {
        return decodeURIComponent(pair[1]);
      }
    }
    console.log('Query variable %s not found', variable);
  };

  toAbsoluteUrl = (convertUrl: string, baseUrl: string): string => {
    baseUrl = baseUrl || (this.getQueryVariable('backend') ?? "localhost");

    if (!convertUrl || !convertUrl.startsWith('/cache')) {
      return convertUrl;
    }

    const httpBackendUrl = baseUrl.replace('ws://', 'http://').replace('wss://', 'https://');
    const components = httpBackendUrl.split('/');

    return components[0] + '//' + components[2] + convertUrl;
  }
}


const LBS = new AppInit();

function App() {
  const [count, setCount] = useState(0)

  const [champSelectState, setChampSelectState] = useState<IChampSelectState>();



  useEffect(() => {
    //prevent fucking double calling
    if (LBS.socket) {
      return;
    }

    const onMessage = function (message: any) {
      switch (message.eventType) {
        case "newState":
          let state = JSON.parse(message.state) as IChampSelectState;
          setChampSelectState(state);
          break;
        default:
          break;
      }
    };

    const connect = (backend: string) => {
      LBS.socket = new WebsocketBuilder(backend)
        .onOpen((i, ev) => {
          console.log('[PB] Connection established!');
        })
        .onClose((i, ev) => {
          setTimeout(connect, 500);
          console.log('[PB] Attempt reconnect in 500ms');
        })
        .onMessage((i, ev) => {
          const data = JSON.parse(ev.data);
          // Maybe check if heartbeat arrives regularly to assure that connection is alive?

          if (data.eventType) {
            onMessage(data);
          } else {
            console.log('[PB] Unexpected packet format: ' + JSON.stringify(data));
          }
        })
        .onError((i, ev) => {
        })
        .build();
    }

    const start = () => {
      LBS.backend = LBS.getQueryVariable("backend") ?? "ws://localhost:9001/pregameSocket";

      console.log(`[LB] Connecting to ws backend on ${LBS.backend}`);

      connect(LBS.backend);
    }


    start();

  }, []);

  return (
    <div className="App">
      <div>
        <a href="https://vitejs.dev" target="_blank">
          <img src="/vite.svg" className="logo" alt="Vite logo" />
        </a>
        <a href="https://reactjs.org" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </div>
  )
}

export default App
