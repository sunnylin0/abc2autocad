


class xtaff {
    constructor(data) {
        this.x = data.x;
        this.y = data.y;
    }
}
class sond {
    parLines;
    type;
    wav;
    constructor(data) {
        this.type = data.type;
        this.wav = data.wav;
        this.parLines = data.parLines;
    }
}
class lines {
    staffs;
    sonditem;
    constructor(data) {
        this.staffs = data.staffs;
        this.sonditem = data.sonditem;
    }
}
var starr = [new xtaff({ x: 75, y: 16 }), new xtaff({ x: 35, y: 86 })]

var sdd = new sond({
    wav: ['sser', 3, 4, 'aaer'],
    type: 'ID2847',
    //parLines: saee
})
var saee = new lines({ staffs: starr, sonditem: sdd })
saee.sonditem.parLines = saee;

let ttm = {
    at: saee,
    ui: 'ui',
    type: 'doele',
}
let ttm123 = {
    e: [{ e1: 43, e2: 53 }, { e4: 44, e5: 55 }, { e3: "ss", e4: 12 }],
    a1: new lines({ staffs: starr, sonditem: sdd }),
    a2: { t1: sdd, t2: sdd, t3: sdd },
    a: ['stri', new lines({ staffs: starr, sonditem: sdd }), [13, 'adsf', 44]],
    f1: (at) => { let cc = 55 + at },
    b: undefined,
    c: [null],
    c1: [null, null, null, null],
    d: {
        uu: 'ss',
        zz: 33
    },
    en: [73, 84, 95],
    ea: [[{ sdf1: 'aaa', sdf2: 'bbb', sdf3: 'ccc' }]],
    es: ['aa', 372, 'asdfw'],
    f: [],
    g: null,
    j: true,
    h: 'aaabbcc',
    i: [{}],
    "123": 'ewr',
    "-1": 'asw'
}

let isNumber = (vl) => {
    if (isNaN(Number(vl)))
        return false
    else
        return true
}

let objkeys = (aa, last = 0) => {
    if (last >= 10) return 'last_10'
    const keyNames = Object.keys(aa).reduce((tol, key) => {
        let tKey = ''
        if (isNumber(key))
            tKey = `"${key}"`;
        else
            tKey = key;
        console.log(Array(last).join('  ') + `${tKey} ${aa[key]} `)
        const objName = Object.prototype.toString.call(aa[key]);

        if (objName.indexOf('Object') > 0) {
            const tmp = ` ${tKey}?:{${objkeys(aa[key], last + 1)}};`
            return tol + '\n' + tmp;
        } else if (objName.indexOf('Array') > 0) {
            if (aa[key].length > 0) {
                const isN = Object.prototype.toString.call(aa[key][0]);
                if (isN.indexOf('Object') > 0) {
                    return tol + '\n' + `${tKey}?:{${objkeys(aa[key][0], last + 1)}}[];`;
                } else if (isN.indexOf('Array') > 0) {
                    return tol + '\n' + `${tKey}?:{${objkeys(aa[key][0][0], last + 1)}}[][];`;
                } else if (isN.indexOf('Null') > 0) {
                    return tol + '\n' + `${tKey}?:[];`;
                } else {
                    return tol + '\n' + `${tKey}?:${typeof (aa[key][0])}[];`;
                }
            } else
                return tol + '\n' + `${tKey}?:[];`;
        } else if (objName.indexOf('Function') > 0) {
            return tol + '\n' + `${tKey}?: ()=>{};`
        } else if (objName.indexOf('Null') > 0) {
            return tol + '\n' + `${tKey}?;`
        } else if (objName.indexOf('Undefined') > 0) {
            return tol + '\n' + `${tKey}?:string;`
        } else {
            return tol + '\n' + `${tKey}?:${typeof (aa[key])};`
        }

    }, '')
    return keyNames;
}

let oks2 = (aa) => { return `type root${parseInt((Math.random() * 100))}={${objkeys(aa)}};` }

let chackValue = (value) => {
    if (value) {
        const N = value.constructor.name;
        if (N == 'Object') {
            return;
        } else if (N == 'Array') {
            return;
        } else if (N == 'Number') {
            return 'number';
        } else if (N == 'Boolean') {
            return 'boolean';
        } else if (N == 'String') {
            return 'string';
        } else {
            return;
        }
    }
    else {
        return;
    }
}

let objKeys2 = (className, data) => {
    //看有沒有迴圈大於 10 次
    if (!mapIdPos[className]) mapIdPos[className] = 0;
    else if (mapIdPos[className] > 10) retrun;
    else mapIdPos[className]++;

    console.log('okjKeys')
    Object.entries(data).forEach(([_k, value]) => {
        if (!mapData[className]) mapData[className] = {};

        if (value) {
            console.log(`${_k} ${value}=>${value.constructor.name} `)
            const N = value.constructor.name;
            if (N == 'Object') {
                const getProp = Object.entries(value).reduce((acc, [_bl, val]) => {
                    acc[_bl] = chackValue(val);
                    return acc;
                }, {})
                mapData[className][_k] = getProp;
            } else if (N == 'Array') {
                if (!chackValue(value))
                    objKeys(className, value[0])

                mapData[className][_k] = value;


            } else if (N == 'Number') {
                mapData[className][_k] = chackValue(value);
                return chackValue(value);
            } else if (N == 'Function') {
                mapData[className][_k] = `()=>{}`;
                return `()=>{}`;
            } else if (N == 'Boolean') {
                mapData[className][_k] = chackValue(value);
                return chackValue(value);
            } else if (N == 'String') {
                mapData[className][_k] = chackValue(value);
                return chackValue(value);
            } else {
                mapData[className][_k] = `__${N}`;
                objKeys(`__${N}`, value)
            }

            //mapData[className][_k] = value;
        }
        else {
            console.log(`${_k} ${value}`)
            mapData[className][_k] = 'string';
        }


    });

}



let mapIdPos = []
let mapData = []
let LOOP_MAX = 5
/**
 * 重覆及空白 資料不 copy
 * @param {any} target
 * @param {any} getObject
 */
function assign2(target, getObject) {
    'use strict';
    if (target == null) { // Attention 2
        throw new TypeError('Cannot convert undefined or null to object');
    }

    var to = Object(target);

    if (getObject != null) {
        for (var nextKey in getObject) {
            console.log(`getObject->`)
            console.log(getObject)
            console.log(`nextKey-> ${nextKey}`)

            let N = getObject[nextKey]?.constructor?.name
            if (!Object.prototype.hasOwnProperty.call(to, nextKey)) {
                to[nextKey] = getObject[nextKey];
            } else if (N == 'Array' && to[nextKey].constructor.name == 'Array') {
                getObject[nextKey].map((tto) => {
                    if (!to[nextKey].some(ths => ths == tto))
                        to[nextKey].push(tto)
                })
            } else if (!isEmpty(getObject[nextKey])) {
                to[nextKey] = getObject[nextKey];
            }
        }
    }
    return to;
}


function isEmpty(obj) {
    if (obj == null || obj === undefined) return true
    let N = obj.constructor.name;
    if (N == 'Object') {
        for (var key in obj) return false

        return true
    } else if (N == 'Array') {
        if (obj.length > 0) return false
        return true
    } else if (N == 'String') {
        if (obj != '') return false
        return true
    } else
        return false;
}

let objKeys = (data, lastPos = 0) => {
    //看有沒有迴圈大於 10 次

    //if (lastPos > 10) return { type: 'Ten', mat: `` };

    //console.log(lastPos)

    //if (chackValue(data)) return chackValue(data);

    if (data == null || data === undefined) return { type: 'Null', mat: '' };

    const N = data.constructor.name;

    console.log(Array(lastPos).join('  ') + `${lastPos} _ ${N}=>${data} `)

    if (N == null || N === undefined) return { type: 'Null', mat: '' };

    if (N == 'Object') {
        if (lastPos > LOOP_MAX) return { type: 'Object', mat: '' };
        lastPos++;
        const mData = Object.entries(data).reduce((acc, [_k, value]) => {
            if (_k == 'a') {
                let cc = 3;
            }
            acc[_k] = objKeys(value, lastPos).mat;
            return acc;
        }, {});
        return { type: 'Object', mat: mData };
    } else if (N == 'Array') {
        if (lastPos > LOOP_MAX) return { type: 'Array', mat: '' };
        lastPos++;
        const mData = Object.values(data).reduce((acc, val, idx) => {
            const ret = objKeys(val, lastPos)
            if (ret.type == 'Object') {
                let ele = acc.find(ths => ths.constructor.name == 'Object')
                if (ele)
                    ele = Object.assign(ele, ret.mat)
                else
                    acc.push(ret.mat);
            } else if (ret.type == 'Array') {
                acc.push(ret.mat);
                //if (idx == 0)                    acc = ret.mat;
                //const ret2 = objKeys(val)
                //acc[ret2.mat] = [ret2.mat]
            } else {
                if (!acc.some(ths => ths == ret.mat))
                    acc.push(ret.mat)
            }

            return acc;
        }, []);

        return { type: 'Array', mat: mData };

    } else if (N == 'Function') {
        return { type: 'Function', mat: `()=>{}` };
    } else if (N == 'Number') {
        return { type: 'Number', mat: 'number' };
    } else if (N == 'Boolean') {
        return { type: 'Boolean', mat: 'boolean' };
    } else if (N == 'String') {
        return { type: 'String', mat: 'string' };
    } else if (N == 'Windows') {
        return { type: 'Windows', mat: 'windows' };
    } else {
        let isOne = false;//是否為第一次

        const _name = `__${N}`;
        if (!Object.prototype.hasOwnProperty.call(mapData, _name)) {
            mapData[_name] = {};
            isOne = true;

        }

        if (N == 'AbsoluteElement' && lastPos == 9) {
            let i = 33;
        }

        if (_name && !(mapIdPos[_name] >= 0)) {
            mapIdPos[_name] = 0
        } else if (mapIdPos[_name] > (LOOP_MAX / 2)) {
            return { type: 'Class', mat: _name };
        }


        if (N == 'SVGPathElement')
            console.log('0000 SVGPathElement')

        if ((lastPos > LOOP_MAX) && isOne == false)
            return { type: 'Class', mat: _name };

        if (N == 'SVGPathElement')
            console.log('11111 SVGPathElement')

        lastPos++;
        const mData = Object.entries(data).reduce((acc, [_k, value]) => {
            const dt = objKeys(value, lastPos).mat
            if ((acc[_k] == null || acc[_k] === undefined))
                acc[_k] = dt;
            else if (acc[_k] === '' && dt != '')
                acc[_k] = dt;

            return acc;
        }, {});

        if (N == 'SVGPathElement') {
            console.log('2222 SVGPathElement  ==>')
            console.log(mData)
        }
        mapData[_name] = assign2(mapData[_name], mData);

        mapIdPos[_name]++;

        //mapData[_name] = Object.assign(mapData[_name], mData);
        return { type: 'Class', mat: _name };



    }

    return { type: 'Null', mat: '' };
}


/** 轉換 JSON 文字檔改成 let 文字 */
function listObj(obj) {
    if (obj == null || obj === undefined) return '';
    const N = obj.constructor.name;

    if (N == null || N === undefined) return '';
    if (N == 'Object') {
        const mData = Object.entries(obj).reduce((acc, [_k, value]) => {
            if (_k == value) {
                acc.push(`${_k}`);
                return acc
            }

            _k = convertSpecialObjectName(_k)
            let tmp = '';
            let md = listObj(value)
            if (md == '')
                tmp = ` ${_k}?`;
            else
                tmp = ` ${_k}?:${md}`;

            acc.push(tmp);

            return acc
        }, []);
        const strDt = mData.join(';\n');
        if (mData.length == 0) {
            return ``;
        } else if (mData.length == 1) {
            //有 : 分號就加 {} 括弧
            if (strDt.indexOf(":") > 1)
                return `{${strDt}}`;
            else
                return `${strDt}`;
        } else if (mData.length > 1)
            return `{\n${strDt};\n}`;

        //    if (mData.indexOf(":") > 1)
        //        return `{${mData}}`;
        //    else
        //        return `${mData}`;
        //} else if (mData.length > 1)
        //    return `{\n${mData.join(';\n')};\n}`;
    } else if (N == 'Array') {
        if (obj.length == 1) {
            const o1 = obj[0];
            const isN = o1?.constructor?.name;
            if (isN == 'Object') {

                return `${listObj(o1)}[]`;
            } else if (isN == 'Null') {
                return `[]`;
            } else {
                return `${listObj(o1)}[]`;
            }
        }
        else if (obj.length > 1) {
            const mData = obj.map((ths) => {
                const N = ths.constructor.name;
                if (N == 'Object') {
                    return listObj(ths);
                } else if (N == 'Array') {
                    return listObj(ths);
                } else
                    return ths
            })
            return `(${mData.join('|')})[]`;

        } else
            return `[]`;
    } else if (N == 'String') {
        return obj;
    }

    else if (N == 'Number')
        return obj;
    else
        return obj;
}



let oks = (aa) => {
    mapData = {};
    mapIdPos = {};
    mapData['root' + parseInt((Math.random() * 100))] = objKeys(aa).mat;
    console.log(mapData)
    let st = '';

    if (mapData == null || mapData === undefined) return '';
    const N = mapData.constructor.name;
    if (N == 'Object') {
        const mData = Object.entries(mapData).reduce((acc, [_k, value]) => {
            if (!isNaN(Number(_k.constructor.name))) _k = `"${_k}"`
            const tmp = `type ${_k}=${listObj(value)}`
            return acc + '\n' + tmp;
        }, '');
        st += `${mData}`;
        if (document.querySelector('#listData'))
            listData.value = st;
        else
            return st
    }

}


function listKeys(arr) {
    if ((typeof arr) == 'Array') {
        arr.map((ths) => {
            return listObj(ths)
        })
    }
}




/**
 * 看物件名 是數字或 特別字就加引號
 * obj-name -> 'obj-name' */
function convertSpecialObjectName(objName) {
    if (!isNaN(Number(objName))) objName = `'${objName}'` //看 key 是不是數字

    const reg = /[-:\[\]]/
    //看 _key  有沒有` - : [ ] `文字 ,有的話就用 引號
    if (reg.test(objName)) objName = `'${objName}'`
    return objName
}


/** 轉換 JSON 文字檔改成 let 文字 */
function convertObjecttoString(obj) {
    if (obj == null || obj === undefined) return obj;
    const N = obj.constructor.name;
    if (N == 'Object') {
        const mData = Object.entries(obj).reduce((acc, [_k, value]) => {
            //if (_k == value) {
            //    acc.push(`${_k}`);
            //    return acc
            //}
            _k = convertSpecialObjectName(_k)
            let tmp = '';
            let md = convertObjecttoString(value)
            if (typeof md === "undefined")
                tmp = ` ${_k}:undefined`;
            else if (!md && typeof (md) !== 'undefined' && md != 0)
                tmp = ` ${_k}:null`;
            else
                tmp = ` ${_k}:${md}`;

            acc.push(tmp);

            return acc
        }, []);

        const strDt = mData.join(',\n');
        if (mData.length == 0) {
            return `{}`;
        } else if (mData.length == 1) {
            //有 : 分號就加 {} 括弧
            if (strDt.indexOf(":") > 1)
                return `{${strDt}}`;
            else
                return `${strDt}`;
        } else if (mData.length > 1)
            return `{\n${strDt}\n}`;

    } else if (N == 'Array') {
        if (obj.length == 1) {
            const o1 = obj[0];
            const isN = o1?.constructor?.name;
            if (isN == 'Object') {
                return `[${convertObjecttoString(o1)}]`;
            } else if (isN == 'Null') {
                return `[]`;
            } else {
                return `[${convertObjecttoString(o1)}]`;
            }
        }
        else if (obj.length > 1) {
            const mData = obj.map((ths) => {
                const N = ths.constructor.name;
                if (N == 'Object') {
                    return convertObjecttoString(ths);
                } else if (N == 'Array') {
                    return convertObjecttoString(ths);
                } else
                    return ths
            })
            return `[${mData.join(',')}]`;

        } else
            return `[]`;
    } else if (N == 'String') {
        let reg = /[\n]/
        // 看文字裡是否有 \n
        if (reg.test(obj))
            return '`' + obj + '`';
        else
            return `'${obj}'`;
    }
    else if (N == null || N === undefined)
        return obj;
    else
        return obj;
}



let CallArr = [[]];
/**
 * 
 * @param { acc: mData, key: _k } parent
 * @param {any} data
 * @param {any} lastPos
 */
function objCall(data, lastPos = 0, key) {
    if (lastPos >= 6) return 'lastPos_10'
    if (data == null || data === undefined) return { type: 'Null', mat: '' };
    const N = data.constructor.name;

    //const mData = {};
    if (N == 'Object') {
        const mData = Object.entries(data).reduce((acc, [_k, value]) => {
            if (_k == 'a') {
                let cc = 3;
            }
            acc[_k] = objCall(value, lastPos + 1).mat;
            return acc;
        }, {});
        return { type: 'Object', mat: mData };


        //if (CallArr[lastPos] == null || CallArr[lastPos] == undefined)
        //    CallArr[lastPos] = []
        //let prt = {}
        //if (!CallArr[lastPos].some(ths => ths.data == data))
        //    CallArr[lastPos].push({ parent: prt, data, key })
        //else
        //    console.log(`no-no`)

        //return { type: 'Object', mat: prt };
    } else if (N == 'Array') {
        // lastPos++;



        const mData = Object.values(data).reduce((acc, val, idx) => {
            const ret = objCall(val, lastPos + 1)
            if (ret.type == 'Object') {
                let ele = acc.find(ths => ths.constructor.name == 'Object')
                if (ele)
                    ele = Object.assign(ele, ret.mat)
                else
                    acc.push(ret.mat);
            } else if (ret.type == 'Array') {
                acc.push(ret.mat);
                //if (idx == 0)                    acc = ret.mat;
                //const ret2 = objKeys(val)
                //acc[ret2.mat] = [ret2.mat]
            } else {
                if (!acc.some(ths => ths == ret.mat))
                    acc.push(ret.mat)
            }

            return acc;
        }, []);

        return { type: 'Array', mat: mData };

    } else if (N == 'Function') {

        return { type: 'Function', mat: `()=>{}` };
    } else if (N == 'Number') {

        return { type: 'Number', mat: 'number' };
    } else if (N == 'Boolean') {

        return { type: 'Boolean', mat: 'boolean' };
    } else if (N == 'String') {

        return { type: 'String', mat: 'string' };

    } else
        if (N == 'Windows') {

            return { type: 'Windows', mat: 'windows' };

        } else {
            const _name = `__${N}`;


            if (!Object.prototype.hasOwnProperty.call(mapData, _name)) {
                mapData[_name] = {};
            }



            if (!CallArr.some(tt => tt.some(ths => ths.data == data))) {
                if (CallArr[lastPos] == null || CallArr[lastPos] == undefined)
                    CallArr[lastPos] = []
                CallArr[lastPos].push({ parent: mapData[_name], data, key: _name })
            }


            //mapData[_name] = assign2(mapData[_name], mData);
            //mapIdPos[_name]++;
            return { type: 'Class', mat: _name };

        }
}
/** 把物件轉換成 TypeScript 的 *.d.ts 物別物件 */
function convertObjectType(data) {
    mapData = {};
    mapIdPos = {};
    CallArr = [[]];
    if (data == null || data === undefined) return '';

    let N = data.constructor.name;
    if (N == 'Object') {
        mapData['root' + parseInt((Math.random() * 100))] = objCall(data).mat
    } else if (N == 'Array') {
        data.forEach((ths, idx) => {
            mapData['mainNumber' + idx] = objCall(ths).mat
        })
    }

    for (let i = 0; i < 10; i++) {
        if (!CallArr[i] || (CallArr[i].length == 0)) continue;

        CallArr[i].map(({ parent, data, key }) => {
            This = CallArr[i];
            if (mapData?.__Tune?.lines?.length > 0)
                console.log(1234)
            if (key == '__Tune')
                console.log(`Tune` + 1234)

            let lstData = {};
            const N = data.constructor.name;
            if (N == 'Object') {
                lstData = Object.entries(data).reduce((acc, [_k, value]) => {
                    //if (!Object.prototype.hasOwnProperty(lstData, _k)) lstData[_k] = ''
                    acc[_k] = objCall(value, i + 1, _k).mat;
                    return acc;
                }, {});

                parent = assign2(parent, lstData)

            } else if (N == 'Array') {
                console.log(234)
            } else {
                lstData = Object.entries(data).reduce((acc, [_k, value]) => {
                    //if (!Object.prototype.hasOwnProperty(lstData, _k)) lstData[_k] = ''
                    acc[_k] = objCall(value, i + 1, _k).mat;
                    return acc;
                }, {});

                parent = assign2(parent, lstData)


            }
        })
    }
    stringifyType(mapData)
}

function stringifyType(mapData) {

    if (mapData == null || mapData === undefined) return '';
    let N = mapData.constructor.name;
    let st = '';
    let mData = {};
    if (N == 'Object' || N == 'Array') {
        mData = Object.entries(mapData).reduce((acc, [_k, value]) => {
            if (!isNaN(Number(_k.constructor.name))) {
                _k = `"${_k}"`
            }
            const tmp = `type ${_k}=${listObj(value)}`
            return acc + '\n' + tmp;
        }, '');
        st += `${mData}`;
        insertDocumentTextArea(st)
        return st;
    }
}
/** 在 document 插入 TextArea */
function insertDocumentTextArea(value) {
    if (document.querySelector('#listData')) {
        let listData = document.querySelector('#listData')
        listData.value = value;
    } else {
        let txtboxAll = document.querySelectorAll('textarea')
        let txtElem = txtboxAll[txtboxAll.length - 1]
        let newTxt = document.createElement('textarea')
        newTxt.id = 'listData';
        txtElem.parentElement.insertBefore(newTxt, txtElem)
        newTxt.value = value;
    }
}
var tt;
function startABC(abcText) {
    //return oks(tt)
    if (ABCJS) {
        let ele = document.createElement('div');
        tt = ABCJS.renderAbc(ele, abcText);
        return convertObjectType(tt)
    }
}

function start() {
    return convertObjectType(ttm123)
}

function JsonReplacerABC2SVG(key, value) {
    //JSON 替換 key value 的( abc2svg用)
    let reValue
    console.log(key)
    switch (key) {
        case 'next': reValue = '__voiceItem'; break;
        case 'prev': reValue = '__voiceItem'; break;
        case 'ts_next': reValue = '__voiceItem'; break;
        case 'ts_prev': reValue = '__voiceItem'; break;
        case 'p_v': reValue = '__Tunes'; break;
        case 'fmt': reValue = '__Formation'; break;
        default: reValue = value; break;
    }
    return reValue;
}


function replacer(key, value) {
    if (key === 'abselem') {
        return ''
    } else if (key === 'abselem') {
        return ''
    } else if (key === 'AbsoluteElement') {
        return ''
    } else if (key === 'RelativeElement') {
        return ''
    } else if (key === 'abselem') {
        return ''
    } else if (key === 'abselem') {
        return ''
    }
    return value;
}
