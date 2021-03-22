import { Scene } from "phaser";
import PlaceholderConversion from "~/PlaceholderConversion";
import IngameScene from "~/scenes/IngameScene";

export default class ItemIndicator {
    icon!: Phaser.GameObjects.Image;
    itemID: number;
    playerID: number;
    animationPhase: number;
    id: string;

    scene: Phaser.Scene;

    constructor(itemData: any, playerID: number, scene: IngameScene ) {
        var spriteLoc = PlaceholderConversion.ConvertItem(itemData).sprite;
        
        this.playerID = playerID;
        this.animationPhase = 0;
        this.scene = scene;
        this.itemID = itemData.itemID;
        this.id = itemData.itemID + '_' + playerID;
        console.log(`Loading Item ${this.id}`);

        scene.load.on(`filecomplete-image-${this.id}`, () => {
          var team = playerID > 4;
          var x = team ? 1881 : 39;
          var y = team ? 189 + ((playerID - 5) * 103) : 189 + (playerID * 103);


          console.log('Loading Item at ' + x + ', ' + y);
          this.icon = scene.add.image(x, y, this.id);
          this.icon.displayHeight = 74;
          this.icon.displayWidth = 78;
          this.icon.setMask(scene.players[playerID]);

          this.icon.setDisplaySize(0,0);
        
          this.showItem();
        });

        scene.load.image(this.id, spriteLoc);
        scene.load.start();
        
    }

    private showItem() {
        var context = this;
        this.scene.tweens.add({
            targets: this.icon,
            props: {
              displayHeight: { value: 74, duration: 400, ease:'Quad.easeInOut'},
              displayWidth: { value: 78, duration: 400, ease:'Quad.easeInOut'}
            },
            paused: false,
            yoyo: false,
            onComplete: function() {context.hideItem();}
          });
    }

    private hideItem(){
        var scene = this.scene;
        var icon = this.icon;
        var ctx = this;
        this.scene.tweens.add({
            targets: icon,
            props: {
              displayHeight: { value: 174, duration: 400, ease:'Quad.easeIn'},
              displayWidth: { value: 178, duration: 400, ease:'Quad.easeIn'},
              alpha: { value: 0, duration: 400, ease: 'Quad.easeIn'}
            },
            paused: false,
            yoyo: false,
            delay: 2800,
            onComplete: function() { icon.destroy(); scene.textures.remove(ctx.id + '');}
          });
    }
}