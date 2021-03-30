import Phaser from 'phaser'
import WebFontLoaderPlugin from 'phaser3-rex-plugins/plugins/webfontloader-plugin.js';
import UIPlugin from 'phaser3-rex-plugins/templates/ui/ui-plugin.js';

import IngameScene from './scenes/IngameScene';

const config: Phaser.Types.Core.GameConfig = {
	type: Phaser.WEBGL,
	width: 1920,
	height: 1080,
	parent: 'gameContainer',
	transparent: true,
	scene: [IngameScene],
	callbacks: {
		postBoot: function (game) {
			// In v3.15, you have to override Phaser's default styles
			game.canvas.style.width = '100%';
			game.canvas.style.height = '100%';
		}
	},
	plugins: {
        global: [{
            key: 'rexWebFontLoader',
            plugin: WebFontLoaderPlugin,
            start: true
        },
        ],
		scene: [{
            key: 'rexUI',
            plugin: UIPlugin,
            mapping: 'rexUI'
        },
        ]
    }
}

export default new Phaser.Game(config)
