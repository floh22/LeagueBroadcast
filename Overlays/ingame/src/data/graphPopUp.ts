import UIPlugin from 'phaser3-rex-plugins/templates/ui/ui-plugin.js';
import { Chart } from 'phaser3-rex-plugins/templates/ui/ui-components.js';
import variables from '~/variables';
import GoldEntry from './goldEntry';
import RemoveChild from 'phaser3-rex-plugins/plugins/gameobjects/containerlite/RemoveChild';
import IngameScene from '~/scenes/IngameScene';


//@ts-ignore
export default class GraphPopUp {

    scene: IngameScene;
    background: Phaser.GameObjects.Image;
    title: Phaser.GameObjects.Text;
    graph: Chart;
    isActive: boolean;
    mask: Phaser.GameObjects.Graphics;
    gridColor: Phaser.Display.Color;

    constructor(scene, data: GoldEntry[]) {

        this.background = scene.add.image(960, 540, 'centerCover');
        this.isActive = false;

        var nameColor = Phaser.Display.Color.IntegerToColor(variables.GOLD_COLOR_PRIMARY);
        var titleColor = Phaser.Display.Color.IntegerToColor(variables.GOLD_COLOR_DARK);
        var dataColor = Phaser.Display.Color.IntegerToColor(variables.GOLD_COLOR_LIGHT);
        this.gridColor = Phaser.Display.Color.IntegerToColor(variables.GOLD_COLOR_DARK);

        this.title = scene.add.text(800, 850, 'GOLD DIFFERENCE OVER ENTIRE GAME', {
            fontFamily: 'News Cycle',
            fontSize: '22px',
            color: GetRGBAString(titleColor, 1),
        });
        this.title.setAlign('center');

        this.mask = scene.make.graphics();
        this.mask.fillStyle(0xffffff);
        //this.mask.fillRect(599,845, 735, 235);
        this.mask.fillRect(-200, 845, 735, 235);
        var tempMask = this.mask.createGeometryMask();

        var values = data.map(a => a.y);

        this.graph = scene.rexUI.add.chart(970, 975, 710, 190, {
            type: 'line',
            data: {
                labels: data.map(entry => entry.x),
                datasets: [
                    {
                        //backgroundColor: GetRGBAString(dataColor, 0.5),
                        //borderColor: GetRGBAString(dataColor, 1),
                        //pointBackgroundColor: GetRGBAString(dataColor, 1),
                        tension: 0.05,
                        fill: 'origin',
                        data: data
                    },
                ]
            },
            plugins: [ColorPlugin],
            options: {
                legend: {
                    display: false,
                },
                elements: {
                    point: {
                        radius: 0
                    }
                },
                scales: {
                    xAxes: [{
                        display: true,
                        type: 'linear',
                        position: 'bottom',
                        gridLines: {
                            color: GetRGBAString(this.gridColor, 0.4),
                            zeroLineWidth: 1,
                            zeroLineColor: GetRGBAString(this.gridColor, 0.3)
                        },
                        ticks: {
                            callback: function (value, index, values) {
                                var minute = Math.floor(parseFloat(value) / 60);
                                var second = parseFloat(value) % 60;
                                if (minute % 5 == 0 && second < 5) return (minute).toFixed(0);
                                else if(index === values.length - 1 && minute % 5 != 0) return (minute).toFixed(0);
                                return '';
                            },
                            autoSkip: true,
                            stepSize: 30,
                            beginAtZero: false,
                            fontColor: GetRGBAString(this.gridColor, 1),
                            fontFamily: 'News Cycle',
                            maxRotation: 0,
                            minRotation: 0
                        }
                    }],
                    yAxes: [{
                        display: true,
                        gridLines: {
                            display: false
                        },
                        ticks: {
                            min: Math.trunc(Math.min.apply(this, values)),
                            max: Math.trunc(Math.max.apply(this, values)),
                            callback: function (value, index, values) {
                                if (index === values.length - 1) return FormatGold(Math.trunc(Math.min.apply(this, values)));
                                else if (index === 0) return FormatGold(Math.trunc(Math.max.apply(this, values)));
                                else if (value == 0) return 0;
                                else return '';
                            },
                            beginAtZero: true,
                            fontColor: GetRGBAString(this.gridColor, 1),
                            fontFamily: 'News Cycle',
                            showLabelBackdrop: false,
                            drawBorder: true
                        }
                    }]
                }
            }
        });

        this.background.setMask(tempMask);
        this.title.setMask(tempMask);
        this.graph.setMask(tempMask);

        this.scene = scene;

    }

    Enable(): void {
        if (this.isActive)
            return;
        this.isActive = true;
        //this.background.setTint(Phaser.Display.Color.RGBStringToColor(this.scene.state!.uiColor).color);
        this.scene.tweens.add({
            targets: this.mask,
            props: {
                x: { value: '+= 799', duration: 1000, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: 1000
        });
    }

    Disable(): void {
        if (!this.isActive)
            return;
        this.isActive = false;
        this.scene.tweens.add({
            targets: this.mask,
            props: {
                x: { value: '-= 799', duration: 1000, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: 1000
        });
    }

    Update(data: GoldEntry[]): void {
        if (data.length === 0) {
            this.Disable();
            return;
        }
        if (data.length !== 0 && !this.isActive) {
            this.Enable();
        }

        var last = data[data.length -1];
        if(last.x % 30 !== 0) {
            data.push(new GoldEntry(Math.ceil(last.x / 30) * 30, last.y));
        }

        //data = SmoothGoldValues(data, 0.85);
        data.unshift(new GoldEntry(data[0].x, data[0].y));
        var values = data.map(a => a.y);
        var min = Math.trunc(Math.min.apply(this, values));
        var max = Math.trunc(Math.max.apply(this, values));
        this.graph.chart.config.data.datasets[0].data = data;
        this.graph.chart.config.options.scales.yAxes[0].ticks = {
            min: min,
            max: max,
            callback: function (value, index, values) {
                if (index === values.length - 1) return FormatGold(min);
                else if (index === 0) return FormatGold(max);
                else if (value == 0 && (Math.abs(max + min) > 2000 && max < 500 || min < 500)) return 0;
                else return '';
            },
            beginAtZero: true,
            fontColor: GetRGBAString(this.gridColor, 1),
            fontFamily: 'News Cycle',
            showLabelBackdrop: false,
            drawBorder: true,
            maxRotation: 0,
            minRotation: 0
        }
        this.graph.chart.update();
    }

}

var GetRGBAString = function (color, alpha) {
    if (alpha === undefined) {
        alpha = color.alphaGL;
    }
    return 'rgba(' + color.red + ',' + color.green + ',' + color.blue + ',' + alpha + ')';
}

const blueRGB = GetRGBAString(Phaser.Display.Color.IntegerToColor(variables.blueColor), 1);
const redRGB = GetRGBAString(Phaser.Display.Color.IntegerToColor(variables.redColor), 1);

var ColorPlugin = {
    beforeRender: function (x, options) {
        var c = x.chart
        var dataset = x.data.datasets[0];
        var yScale = x.scales['y-axis-0'];
        var yPos = yScale.getPixelForValue(0);

        var gradientFill = c.ctx.createLinearGradient(0, 0, 0, c.height);
        gradientFill.addColorStop(0, blueRGB);
        var blue = yPos / c.height - 0.01;
        if (blue > 1)
            blue = 1;
        else if (blue < 0)
            blue = 0;
        gradientFill.addColorStop(blue, blueRGB);
        var red = yPos / c.height + 0.01;
        if (red > 1)
            red = 1;
        else if (red < 0)
            red = 0;
        gradientFill.addColorStop(red, redRGB);
        gradientFill.addColorStop(1, redRGB);

        var model = x.data.datasets[0]._meta[Object.keys(dataset._meta)[0]].dataset._model;
        model.backgroundColor = gradientFill;
    }
}

var FormatGold = function (num) {
    return Math.abs(num) > 999 ? ((Math.abs(num) / 1000).toFixed(1)) + 'k' : Math.abs(num)
}

function avg(v) {
    return v.reduce((a, b) => a + b, 0) / v.length;
}

function SmoothGoldValues(vector: GoldEntry[], variance: number): GoldEntry[] {
    var t_avg = avg(vector.map(a => a.y)) * variance;
    var ret: GoldEntry[] = Array(vector.length);
    for (var i = 0; i < vector.length; i++) {
        (function () {
            var prev = i > 0 ? ret[i - 1].y : vector[i].y;
            var next = i < vector.length ? vector[i].y : vector[i - 1].y;
            ret[i] = new GoldEntry(vector[i].x, avg([t_avg, avg([prev, vector[i].y, next])]));
        })();
    }
    return ret;
}