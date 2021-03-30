import 'phaser';
import IngameScene from '~/scenes/IngameScene';
import FrontEndObjective from './frontEndObjective';

export default class ObjectiveIndicator {
  backgroundBox: Phaser.GameObjects.Image;
  icon: Phaser.GameObjects.Image;
  time: Phaser.GameObjects.Text;
  cdrIcon: Phaser.GameObjects.Image;
  gold: Phaser.GameObjects.Text;
  goldIcon: Phaser.GameObjects.Image;
  swipeRight: boolean;
  dirMult: number;
  isActive: boolean = false;
  id: string;

  scene: IngameScene;

  mask: Phaser.Display.Masks.BitmapMask;

  x: number;
  y: number;

  constructor(id: string, x: number, y: number, scene: IngameScene, icon: string, backgroundBox: string, time: string, gold: number, swipeRight: boolean) {
    this.scene = scene;
    this.id = id;

    this.x = x;
    this.y = y;

    this.dirMult = swipeRight ? 1 : -1;
    this.swipeRight = swipeRight;

    //All of this positioning is giga janky.... im sorry to myself incase I ever want to change this
    //Change default pos based on orientation
    var xPos = swipeRight ? x - (180 * this.dirMult) : x - (130 * this.dirMult);

    //Background
    this.backgroundBox = scene.add.image(x - (200 * this.dirMult), y, backgroundBox);

    //Time Icon and Text
    this.time = scene.add.text(swipeRight? xPos + 35 : xPos + 50, y - 29, time, {
      fontFamily: 'News Cycle',
      fontSize: '22px',
      fontStyle: 'Bold',
      color: '#E6BE8A'
    });
    this.time.setOrigin(1,0)
    this.time.setAlign('right');
    this.cdrIcon = scene.add.image(xPos + (swipeRight ? 53 : 68), y - 16, 'cdr');


    //Gold Icon and Text
    var goldText = gold + '';
    this.gold = scene.add.text(swipeRight? xPos + 35 : xPos + 50, y, goldText, {
      fontFamily: 'News Cycle',
      fontSize: '22px',
      fontStyle: 'Bold',
      color: '#E6BE8A'
    });
    this.gold.setOrigin(1, 0);
    this.gold.setAlign('right');

    //Gold Icon
    var goldIconPos = x + (swipeRight ? -125 : 200);
    this.goldIcon = scene.add.image(goldIconPos, y + 15, 'goldIcon');

    //Object Mask
    var maskImage = scene.make.sprite({ x: x + (75 * this.dirMult), y: y, key: 'objectiveMask', add: false });
    maskImage.setScale(0.8);
    this.mask = maskImage.createBitmapMask();

    //Objective Icon
    this.icon = scene.add.image(x, y, icon);
    this.icon.alpha = 0;

    //Update Scale
    this.backgroundBox.setScale(0.8);
    this.icon.setScale(0.8);

    //Set Masks
    this.backgroundBox.mask = this.mask;
    this.gold.mask = this.mask;
    this.time.mask = this.mask;
    this.goldIcon.mask = this.mask;
    this.cdrIcon.mask = this.mask;

  }

  updateContent = (objective: FrontEndObjective) => {
    if(objective === undefined || objective === null ) {
      if(this.isActive) {
        this.hideContent();
      }
      return;
    }
    if(!this.isActive) {
      this.showContent();
    }
    console.log(`Updating ${this.id} content time: ${objective.DurationRemaining}, gold: ${objective.GoldDifference}`);
    this.gold.text = objective.GoldDifference + '';
    this.time.text = objective.DurationRemaining;
  }


  showContent = () => {
    if(this.isActive) {
      return;
    }
    console.log("Showing objective content");
    this.isActive = true;
    var ctx = this;
    this.scene.tweens.add({
      targets: this.icon,
      alpha: 1,
      paused: false,
      yoyo: false,
      duration: 250,
      onComplete: function() {ctx.scene.tweens.add({
        targets: [ctx.backgroundBox, ctx.gold, ctx.time, ctx.goldIcon, ctx.cdrIcon],
        props: {
          x: { value: '+=' + 200 * ctx.dirMult, duration: 1000, ease: 'Cubic.easeOut' }
        },
        paused: false,
        yoyo: false,
      });}
    });

  }

  hideContent = () => {
    if(!this.isActive) {
      return;
    }
    
    var icon = this.icon;
    var bg = this.backgroundBox;
    var gold = this.gold;
    var time = this.time;
    var cdrIcon = this.cdrIcon;
    var goldIcon = this.goldIcon;
    var ctx = this;

    this.scene.tweens.add({
      targets: icon,
      alpha: 0,
      paused: false,
      yoyo: false,
      duration: 200,
      delay: 501,
      onComplete: function () { }
    });

    this.scene.tweens.add({
      targets: [bg, gold, time, goldIcon, cdrIcon],
      props: {
        x: { value: '-=' + 200 * this.dirMult, duration: 1000, ease: 'Cubic.easeOut' }
      },
      paused: false,
      yoyo: false,
      onComplete: function() { ctx.isActive = false;}
      //onComplete: function () { bg.destroy(); gold.destroy(); time.destroy(); goldIcon.destroy(); cdrIcon.destroy(); }
    });
  }
}