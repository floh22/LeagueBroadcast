import { Chart } from 'phaser3-rex-plugins/templates/ui/ui-components.js';
import { GoldGraphDisplayConfig, GraphDisplayConfig } from "~/data/config/overlayConfig";
import GoldEntry from '~/data/goldEntry';
import IngameScene from "~/scenes/IngameScene";
import ColorUtils from '~/util/ColorUtils';
import variables from '~/variables';
import { VisualElement } from "./VisualElement";

export default class GraphVisual extends VisualElement {

    BackgroundRect: Phaser.GameObjects.Rectangle | null = null;
    BackgroundImage: Phaser.GameObjects.Image | null = null;
    BackgroundVideo: Phaser.GameObjects.Video | null = null;
    ImgMask!: Phaser.Display.Masks.BitmapMask;
    GeoMask!: Phaser.Display.Masks.GeometryMask;
    MaskImage!: Phaser.GameObjects.Sprite;
    MaskGeo!: Phaser.GameObjects.Graphics;

    Title: Phaser.GameObjects.Text | null = null;
    Graph: Chart;

    CurrentData: GoldEntry[] = [];

    constructor(scene: IngameScene, cfg: GoldGraphDisplayConfig, data: GoldEntry[]) {
        super(scene, cfg.Position, "scoreboard");
        this.CurrentData = data;

        this.CreateTextureListeners();

        //Mask
        if (cfg.Background.UseAlpha) {
            this.MaskImage = scene.make.sprite({ x: cfg.Position.X - cfg.Size.X * 1.5, y: cfg.Position.Y, key: 'graphMask', add: true});
            this.MaskImage.setDisplaySize(cfg.Size.X, cfg.Size.Y);
            this.ImgMask = this.MaskImage.createBitmapMask();
        } else {
            this.MaskGeo = scene.make.graphics({add: false});
            this.MaskGeo.fillStyle(0xffffff);
            this.MaskGeo.fillRect(0, 0, cfg.Size.X, cfg.Size.Y);
            this.GeoMask = this.MaskGeo.createGeometryMask();
            this.MaskGeo.setPosition(cfg.Position.X - cfg.Size.X * 1.5, cfg.Position.Y);
        }

        //Background
        if (cfg.Background.UseVideo) {
            this.scene.load.video('graphBgVideo', 'frontend/backgrounds/GoldGraph.mp4');
        } else if (cfg.Background.UseImage) {
            this.scene.load.image('graphBg', 'frontend/backgrounds/GoldGraph.png');
        } else {
            this.BackgroundRect = this.scene.add.rectangle(cfg.Position.X, cfg.Position.Y, cfg.Size.X, cfg.Size.Y, Phaser.Display.Color.RGBStringToColor(cfg.Background.FallbackColor).color);
            this.BackgroundRect.setOrigin(0.5,0);
            this.BackgroundRect.depth = -1;
            this.AddVisualComponent(this.BackgroundRect);
        }

        //Title
        if(cfg.Title.Enabled) {
            this.Title = scene.add.text(cfg.Position.X + cfg.Title.Position.X, cfg.Position.Y + cfg.Title.Position.Y, cfg.Title.Text, {
                fontFamily: cfg.Title.Font.Name,
                fontSize: cfg.Title.Font.Size,
                color: cfg.Title.Font.Color,
                fontStyle: cfg.Title.Font.Style,
                align: cfg.Title.Font.Align
            });
            this.Title.setOrigin(0.5,0);
            this.AddVisualComponent(this.Title);
        }

        //Set Mask
        this.GetActiveVisualComponents().forEach(vc => {
            vc.setMask(GraphVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
        });


        //Load Resources
        if (cfg.Background.UseImage || cfg.Background.UseVideo) {
            this.scene.load.start();
        }

        this.Load();
    }
    UpdateValues(newValues: GoldEntry[]): void {
        if(newValues.length === 0) {
            if (this.isActive && !this.isHiding) {
                this.Stop();
            }
            return;
        }
        if (newValues.length !== 0 && !this.isActive && !this.isShowing) {

            //this.Start();
            this.scene.displayRegions[10].AddToAnimationQueue(this);
        }

        this.CurrentData = newValues;
        let goldValues = this.CurrentData.map(a => a.y);
        this.Graph.chart.config.data.datasets[0].data = this.CurrentData;
        let ctx = this;
        
        this.Graph.chart.config.options.scales.y.ticks = {
            min: Math.trunc(Math.min(...goldValues)),
            max: Math.trunc(Math.max(...goldValues)),
            callback: function (value, index, values) {
                
                if (index === values.length - 1) return GraphVisual.FormatGold(Math.trunc(Math.max(...goldValues)));
                else if (index === 0) return GraphVisual.FormatGold(Math.trunc(Math.min(...goldValues)));
                else if (value == 0) return 0;
                else return null;
            },
            beginAtZero: true,
            font: {
                family: GraphVisual.GetConfig().Graph.InfoFont.Name,
            },
            color: GraphVisual.GetConfig().Graph.InfoFont.Color,
            drawBorder: true,
            maxRotation: 0,
            minRotation: 0
        }
        
        this.Graph.chart.update();
    }
    UpdateConfig(cfg: GoldGraphDisplayConfig): void {

        //Delete the Graph and make a new one
        if(this.Graph !== null && this.Graph !== undefined)
            this.Graph.destroy();
        this.RemoveVisualComponent(this.Graph);

        //Position
        this.position = cfg.Position;

        //Background
        if (!cfg.Background.UseAlpha) {
            this.MaskGeo.clear();
            this.MaskGeo.fillStyle(0xffffff);
            this.MaskGeo.fillRect(0, 0, cfg.Size.X, cfg.Size.Y);
            this.MaskGeo.setPosition(cfg.Position.X - cfg.Size.X * 1.5, cfg.Position.Y);
        } else {
            this.MaskImage.setDisplaySize(cfg.Size.X, cfg.Size.Y);
            this.MaskImage.setPosition(cfg.Position.X - cfg.Size.X * 1.5, cfg.Position.Y)
        }

        //Background Image
        if (cfg.Background.UseImage) {
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo.destroy();
            }
            if (this.BackgroundRect !== undefined && this.BackgroundRect !== null) {
                this.RemoveVisualComponent(this.BackgroundRect);
                this.BackgroundRect.destroy();
                this.BackgroundRect = null;
            }
            //Reset old Texture
            if (this.scene.textures.exists('graphBg')) {
                this.RemoveVisualComponent(this.BackgroundImage);
                this.BackgroundImage?.destroy();
                this.BackgroundImage = null;
                this.scene.textures.remove('graphBg');
            }
            this.scene.load.image('graphBg', 'frontend/backgrounds/GoldGraph.png');
        }
        //Background Video
        else if (cfg.Background.UseVideo) {
            if (this.BackgroundRect !== undefined && this.BackgroundRect !== null) {
                this.RemoveVisualComponent(this.BackgroundRect);
                this.BackgroundRect.destroy();
                this.BackgroundRect = null;
            }
            if (this.BackgroundImage !== undefined && this.BackgroundImage !== null) {
                this.RemoveVisualComponent(this.BackgroundImage);
                this.BackgroundImage.destroy();
                this.BackgroundImage = null;
            }
            //Reset old Video
            if(!this.scene.cache.video.has('graphBgVideo')) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo?.destroy(),
                this.BackgroundVideo = null;
                this.scene.cache.video.remove('graphBgVideo');
            }
            this.scene.load.video('graphBgVideo', 'frontend/backgrounds/GoldGraph.mp4');
        }
        //Background Color
        else {
            if (this.BackgroundImage !== undefined && this.BackgroundImage !== null) {
                this.RemoveVisualComponent(this.BackgroundImage);
                this.BackgroundImage.destroy();
            }
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo.destroy();
                this.BackgroundVideo = null;
            }
            if (this.BackgroundRect === null || this.BackgroundRect === undefined) {
                this.BackgroundRect = this.scene.add.rectangle(cfg.Position.X, cfg.Position.Y, cfg.Size.X, cfg.Size.Y, Phaser.Display.Color.RGBStringToColor(cfg.Background.FallbackColor).color32);
                this.BackgroundRect.setOrigin(0, 0);
                this.BackgroundRect.depth = -1;
                this.BackgroundRect.setMask(GraphVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
                this.AddVisualComponent(this.BackgroundRect);
            }
            this.BackgroundRect.setPosition(cfg.Position.X, cfg.Position.Y);
            this.BackgroundRect.setDisplaySize(cfg.Size.X, cfg.Size.Y);
            this.BackgroundRect.setFillStyle(Phaser.Display.Color.RGBStringToColor(cfg.Background.FallbackColor).color, 1);
        }
        


        //Graph Colors
        var BorderBlueColor = Phaser.Display.Color.IntegerToColor(variables.fallbackBlue);
        if (cfg.Graph.BorderUseTeamColors && this.scene.state?.blueColor !== undefined && this.scene.state.blueColor !== '') {
            BorderBlueColor = Phaser.Display.Color.RGBStringToColor(this.scene.state?.blueColor);
        } else {
            BorderBlueColor = Phaser.Display.Color.RGBStringToColor(cfg.Graph.BorderOrderColor);
        }

        var FillBlueColor = Phaser.Display.Color.IntegerToColor(variables.fallbackBlue);
        if (cfg.Graph.BorderUseTeamColors && this.scene.state?.blueColor !== undefined && this.scene.state.blueColor !== '') {
            FillBlueColor = Phaser.Display.Color.RGBStringToColor(this.scene.state?.blueColor);
        } else {
            FillBlueColor = Phaser.Display.Color.RGBStringToColor(cfg.Graph.FillOrderColor);
        }


        var BorderRedColor = Phaser.Display.Color.IntegerToColor(variables.fallbackRed);
        if (cfg.Graph.BorderUseTeamColors && this.scene.state?.redColor !== undefined && this.scene.state.redColor !== '') {
            BorderRedColor = Phaser.Display.Color.RGBStringToColor(this.scene.state?.redColor);
        } else {
            BorderRedColor = Phaser.Display.Color.RGBStringToColor(cfg.Graph.BorderChaosColor);
        }

        var FillRedColor = Phaser.Display.Color.IntegerToColor(variables.fallbackRed);
        if (cfg.Graph.BorderUseTeamColors && this.scene.state?.redColor !== undefined && this.scene.state.redColor !== '') {
            FillRedColor = Phaser.Display.Color.RGBStringToColor(this.scene.state?.redColor);
        } else {
            FillRedColor = Phaser.Display.Color.RGBStringToColor(cfg.Graph.BorderChaosColor);
        }

        let goldValues = this.CurrentData.map(a => a.y);

        
        //Graph
        this.Graph = this.scene.rexUI.add.chart(cfg.Position.X + cfg.Graph.Position.X, cfg.Position.Y + cfg.Graph.Position.Y, cfg.Graph.Size.X, cfg.Graph.Size.Y, {
            type: 'line',
            data: {
                labels: this.CurrentData.map(entry => entry.x),
                datasets: [
                    {
                        tension: cfg.Graph.LineTension,
                        fill: {
                            target: 'origin',
                            above: ColorUtils.GetRGBAString(FillBlueColor, 1),   // Area will be red above the origin
                            below: ColorUtils.GetRGBAString(FillRedColor, 1)    // And blue below the origin
                        },
                        data: this.CurrentData
                    },
                ]
            },
            options: {
                animation: {
                    duration: 0
                },
                plugins: {
                    legend: {
                        display: false
                    }
                },
                cubicInterpolationMode: 'monotone',
                elements: {
                    point: {
                        radius: 0
                    }
                },
                scales: {
                    x: {
                        display: cfg.Graph.ShowHorizontalGrid,
                        alignToPixels: true,
                        type: 'linear',
                        position: 'bottom',
                        grid: {
                            color: cfg.Graph.GridColor,
                            lineWidth: 1,
                            drawTicks: false,
                            display: cfg.Graph.ShowVerticalGrid
                        },
                        ticks: {
                            callback: function (value, index, values) {
                                var minute = Math.floor(parseFloat(value) / 60);
                                var second = parseFloat(value) % 60;
                                if (minute % 5 == 0 && second === 0 && index !== values.length - 2) return (minute).toFixed(0);
                                else if(index === values.length - 1) return (Math.round(parseFloat(value) / 60)).toFixed(0);
                                if(cfg.Graph.ShowTimeStepIndicators && value % cfg.Graph.TimeStepSize < 1) return '';
                                return null;
                            },
                            autoSkip: true,
                            stepSize: 1,
                            beginAtZero: false,
                            font: {
                                family: cfg.Graph.InfoFont.Name,
                            },
                            color: cfg.Graph.InfoFont.Color,
                            maxRotation: 0,
                            minRotation: 0
                        }
                    },
                    y: {
                        display: true,
                        alignToPixels: true,
                        grid: {
                            color: cfg.Graph.GridColor,
                            lineWidth: 1,
                            drawTicks: false,
                            display: cfg.Graph.ShowHorizontalGrid
                        },
                        ticks: {
                            min: Math.trunc(Math.min(...goldValues)),
                            max: Math.trunc(Math.max(...goldValues)),
                            callback: function (value, index, values) {
                                
                                if (index === values.length - 1) return GraphVisual.FormatGold(Math.trunc(Math.max(...goldValues)));
                                else if (index === 0) return GraphVisual.FormatGold(Math.trunc(Math.min(...goldValues)));
                                else if (value == 0) return 0;
                                else return null;
                            },
                            beginAtZero: true,
                            font: {
                                family: cfg.Graph.InfoFont.Name,
                            },
                            color: cfg.Graph.InfoFont.Color,
                            drawBorder: true,
                            maxRotation: 0,
                            minRotation: 0
                        }
                    }
                }
            }
        });
        this.Graph.setOrigin(0.5,0);
        this.Graph.setMask(GraphVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
        this.AddVisualComponent(this.Graph);

        

        //Title
        if (cfg.Title.Enabled) {
            if (GraphVisual.GetConfig().Title.Enabled) {
                this.Title?.setPosition(cfg.Position.X + cfg.Title.Position.X, cfg.Position.Y + cfg.Title.Position.Y);
                this.UpdateTextStyle(this.Title!, cfg.Title.Font);
            } else {
                this.Title = this.scene.add.text(cfg.Position.X + cfg.Title.Position.X, cfg.Position.Y + cfg.Title.Position.Y, cfg.Title.Text, {
                    fontFamily: cfg.Title.Font.Name,
                    fontSize: cfg.Title.Font.Size,
                    color: cfg.Title.Font.Color,
                    fontStyle: cfg.Title.Font.Style,
                    align: 'center'
                });
                this.Title.setOrigin(0.5, 0);
                this.Title.setMask(GraphVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
                this.AddVisualComponent(this.Title);
            }
        }
        else if (!(this.Title === null || this.Title === undefined)) {
            this.RemoveVisualComponent(this.Title);
            this.Title.destroy();
            this.Title = null;
        }

    }
    Load(): void {
        //Fully loaded in start
    }
    Start(): void {
        if (this.isActive || this.isShowing)
            return;
        this.isShowing = true;
        var ctx = this;
        
        this.UpdateConfig(GraphVisual.GetConfig());

        this.currentAnimation[0] = this.scene.tweens.add({
            targets: [ctx.MaskGeo, ctx.MaskImage],
            props: {
                x: { from: GraphVisual.GetConfig().Position.X - GraphVisual.GetConfig().Size.X * 1.5, to: GraphVisual.GetConfig().Position.X - GraphVisual.GetConfig().Size.X * 0.5, duration: 1000, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: 1000,
            onComplete: function() {
                ctx.isActive = true;
                ctx.isShowing = false;
            }
        });
    }
    Stop(): void {
        if (!this.isActive || this.isHiding)
            return;
        this.isActive = false;
        this.isHiding = true;
        var ctx = this;

        this.currentAnimation[0] = this.scene.tweens.add({
            targets: [ctx.MaskGeo, ctx.MaskImage],
            props: {
                x: { from: GraphVisual.GetConfig().Position.X - GraphVisual.GetConfig().Size.X * 0.5, to: GraphVisual.GetConfig().Position.X - GraphVisual.GetConfig().Size.X * 1.5, duration: 1000, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: 1000,
            onComplete: function() {
                ctx.isHiding = false;
                ctx.AnimationComplete.dispatch();
            }
        });
    }

    static GetConfig(): GoldGraphDisplayConfig {
        return IngameScene.Instance.overlayCfg!.GoldGraph;
    }

    static FormatGold(num: number): string {
        let res = Math.abs(num) > 999 ? ((Math.abs(num) / 1000).toFixed(1)) + 'k' : Math.abs(num) + ''
        return res;
    }
    
    Average(v) {
        return v.reduce((a, b) => a + b, 0) / v.length;
    }

    CreateTextureListeners(): void {
        //Background Image support
        this.scene.load.on(`filecomplete-image-graphBg`, () => {
            this.BackgroundImage = this.scene.make.sprite({ x: GraphVisual.GetConfig().Position.X, y: GraphVisual.GetConfig().Position.Y, key: 'graphBg', add: true });
            this.BackgroundImage.setOrigin(0.5,0);
            this.BackgroundImage.setDisplaySize(GraphVisual.GetConfig().Size.X, GraphVisual.GetConfig().Size.Y);
            this.BackgroundImage.setDepth(-1);
            this.BackgroundImage.setMask(GraphVisual.GetConfig()?.Background.UseAlpha ? this.ImgMask : this.GeoMask);
            this.AddVisualComponent(this.BackgroundImage);
        });

        //Background Video support
        this.scene.load.on(`filecomplete-video-graphBgVideo`, () => {
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo.destroy();
            }
            // @ts-ignore
            this.BackgroundVideo = this.scene.add.video(GraphVisual.GetConfig().Position.X, GraphVisual.GetConfig()!.Position.Y, 'graphBgVideo', false, true);
            this.BackgroundVideo.setDisplaySize(GraphVisual.GetConfig().Size.X, GraphVisual.GetConfig().Size.Y);
            this.BackgroundVideo.setOrigin(0.5,0);
            this.BackgroundVideo.setMask(GraphVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
            this.BackgroundVideo.setLoop(true);
            this.BackgroundVideo.setDepth(-1);
            this.BackgroundVideo.play();
            this.AddVisualComponent(this.BackgroundVideo);
        });
    }

}