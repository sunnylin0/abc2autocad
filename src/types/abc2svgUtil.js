if (!symAll) var symAll = []; else symAll = [];
if (!ts) var ts = []; else ts = [];
if (!tsAll) var tsAll = []; else tsAll = [];
if (!tsType) var tsType = []; else tsType = [];
if (!tsCheckType) var tsCheckType = []; else tsCheckType = [];
if (!tsPropDiffs) var tsPropDiffs = []; else tsPropDiffs = [];
if (!tsPropBase) var tsPropBase = []; else tsPropBase = [];
if (!voitem) {
    var voitem = {
        type: 0, v: 0, dur: 0, time: 0, fmt: 0, p_v: 0, x: 0, y: 0, wr: 0, wl: 0, st: 0, next: 0, prev: 0,
        ts_prev: 0, ts_next: 0
    };
} else {
    voitem = {
        type: 0, v: 0, dur: 0, time: 0, fmt: 0, p_v: 0, x: 0, y: 0, wr: 0, wl: 0, st: 0, next: 0, prev: 0,
        ts_prev: 0, ts_next: 0
    };
}

function reloadAbleJSFn(id, newJS) {
    //重新載入javascript檔案的方法(給js定個id), 自己封裝成一個方法方便大家使用：
    var oldjs = null;
    var t = null;
    var oldjs = document.getElementById(id);
    if (oldjs) oldjs.parentNode.removeChild(oldjs);
    var scriptObj = document.createElement("script");
    scriptObj.src = newJS;
    scriptObj.type = "text/javascript";
    scriptObj.id = id;
    document.getElementsByTagName("head")[0].appendChild(scriptObj);
}

function a2() {
    let qt = document.querySelector('.text-center.question-text');
    let out = document.getElementById('answer-input-math');
    if (!qt) return
    let ans = `${eval(qt.textContent)}`;
    let idans = document.getElementById('idans');
    let pp;
    if (!idans) {
        pp = document.createElement('p')
    } else { pp = idans }
    pp.textContent = ans;
    pp.id = 'idans';
    pp.className = 'text-center';
    qt.parentElement.insertBefore(pp, qt);
    if (!out.value) out.value = ans;
}
function a() {
    let qt = document.querySelector('.text-center.question-text');
    if (!qt) return
    let ans = `${eval(qt.textContent)}`;
    let idans = document.getElementById('idans');
    let pp;
    if (!idans) {
        pp = document.createElement('p');
    } else { pp = idans; }
    pp.textContent = ans;
    pp.id = 'idans';
    pp.className = 'text-center';
    qt.parentElement.insertBefore(pp, qt);
}
//let ii = setInterval(a, 200)

var voiceBaseItem = {}
window.onload = function () {
    layoutabc2svg()
}

function layoutabc2svg() {
    let listTextBox = document.querySelectorAll('textarea')
    let lastElem = listTextBox[listTextBox.length - 1]
    let elemList = [
        { type: 'p' },
        { type: 'button', txt: `重載 abc2svgUtil.js`, fun: loadUtil_js },
        { type: 'p' },
        { type: 'button', txt: `TS 加入 號碼++`, fun: abc2addTS },
        { type: 'button', txt: `get Next() Diff`, fun: abc2getNextDiff },
        { type: 'button', txt: `TS 轉成 *.d.ts`, fun: toTS2_d_ts },
        { type: 'p' },
        { type: 'button', txt: `TS 陣列的交集`, fun: abc2getNextIns },
        { type: 'button', txt: `轉成 autocad 音樂譜`, fun: toAcadMusic },
        { type: 'button', txt: `轉成 autocad 的 json`, fun: toAcadMusicJSON },
        {
            type: 'div', innerHTML: `<input type="checkbox" id="checkedTSdiff" name="checkedTSdiff" checked=checked />
        <label for="scales">要 Diff 第一個元素</label>` },
        { type: 'p', innerHTML: `<textarea id="listData" style="width:350px;height:300px;left:30px"></textarea>` },
    ]
    function layoutAddElement(tag, addElemList) {
        addElemList.forEach((ths) => {
            let e = document.createElement(ths.type)
            if (ths.txt) e.textContent = ths.txt
            if (ths.fun) e.onclick = ths.fun
            if (ths.innerHTML) e.innerHTML = ths.innerHTML
            tag.parentElement.insertBefore(e, tag)
        })
    }
    layoutAddElement(lastElem, elemList)
}
function abc2getNextDiff() {
    // findCheckBaseObject()
    let ischecked = document.getElementById('checkedTSdiff').checked
    TScompareProDifference(ischecked)
}
function abc2getNextIns() {
    TScompareProInserselect()
}
function abc2addTS() {
    getVoicItemOne();
}

function toTS2_d_ts() {
    //清掉 會迴圈的屬性
    let json = JSON.stringify(tt123, JsonReplacerABC2SVG, 2)
    json = JSON.parse(json)

    convertObjectType(json)
}
function loadUtil_js() {
    //重載 abc2svgUtil.js
    reloadAbleJSFn("abcUtil_js", "../src/types/abc2svgUtil.js")
}

if (!isDD_start) var isDD_start = false; else isDD_start = false;
function JsonReplacerABC2SVG(key, value) {
    //JSON 替換 key value 的( abc2svg用)
    let reValue
    switch (key) {
        case 'ss': reValue = '__slsss'; break;
        case 'se': reValue = '__slsse'; break;
        case 'next': reValue = '__voiceItem'; break;
        case 'prev': reValue = '__voiceItem'; break;
        case 'ts_next': reValue = '__voiceItem'; break;
        case 'ts_prev': reValue = '__voiceItem'; break;
        case 'p_v': reValue = '__Tunes'; break;
        case 'fmt': reValue = '__Formation'; break;
        case 'dd_st':
            if (isDD_start) {
                isDD_start = !isDD_start
                reValue = '__DD_start';
            } else {
                isDD_start = !isDD_start
                reValue = value
            }
            break;
        case 'dd_en':
            if (isDD_start) {
                isDD_start = !isDD_start
                reValue = '__DD_end';
            } else {
                isDD_start = !isDD_start
                reValue = value
            }
            break;
        default:
            //if (value && value.constructor.name == 'Object') {
            //    console.log(`rep ${key}:Object`)
            //}
            if (value == null || value === undefined) reValue = value;
            else if (value.constructor.name == 'Number') {//這是看 小數點是否太長
                let sstr = value.toString()
                if ((sstr.length - sstr.indexOf('.')) > 6) {
                    reValue = Number(sstr.slice(0, sstr.indexOf('.') + 4))
                } else
                    reValue = value;
            }
            else
                reValue = value;
            break;
    }
    return reValue;
}


function getVoicItemOne() {
    symLoop()
    ts_nextLoop()
    for (let i = 0; i < 25; i++) {
        tsType[i] = {}
    }
    let tsElem = tsAll[0]
    for (; tsElem; tsElem = tsElem.ts_next) {
        if (!tsElem) break;
        Object.assign(tsType[tsElem.type], tsElem)
    }

}

function symLoop() {
    let elem = tt123[0].sym.next;
    let fireElem;
    for (; elem; elem = elem?.prev) {
        if (!elem?.prev)
            fireElem = elem
    }
    let nn = fireElem;
    let pos = 0
    symAll = []
    for (; nn; nn = nn?.next) {
        symAll.push(nn)
        if (nn.fname) {
            if (nn.fname.indexOf('tsItem') < 0)
                nn.fname = `sym${pos++}`
        }
        else
            nn.fname = `__sym${pos++}`
    }
}

function ts_nextLoop() {
    let elem = tt123[0].sym.next;
    let fireElem;
    for (; elem; elem = elem?.ts_prev) {
        if (!elem?.ts_prev)
            fireElem = elem
    }
    let nn = fireElem;
    let pos = 0
    tsAll = []
    for (; nn; nn = nn?.ts_next) {
        tsAll.push(nn)
        nn.__TSpos = `__tsItem${pos++}`
    }
}
function sym0() {
    let nn = tt123[0].sym;
    let pos = 1
    for (; nn; nn = nn?.next) {
        if (nn.fname)
            nn.fname = `sym${pos++}`
        else
            nn.fname = `__sym${pos++}`
        console.log(`${nn.fname}->type:${nn.type}`)
        console.log(nn)
    }
}
function findCheckBaseObject() {
    //compareProDifference
    //取到最簡單的元素屬性
    let elem = tt123[0].sym.next;
    let fireElem;
    //先找到第一個頭
    for (; elem; elem = elem?.ts_prev) {
        if (!elem?.ts_prev)
            fireElem = elem
    }

    Object.assign(voiceBaseItem, fireElem)
    elem = fireElem;
    // 放入陣列
    for (; elem; elem = elem?.ts_next) {
        let baseKey = Object.keys(voiceBaseItem)
        let tagKey = Object.keys(elem)
        baseKey.forEach((B) => {
            let isFind = tagKey.find((T) => {
                return B == T
            })
            if (!isFind) {
                delete voiceBaseItem[B]
            }
        })
    }
    //取到最簡單的元素屬性
    return voiceBaseItem
}

function checkType() {
    let elem = tt123[0].sym.next;
    let fireElem;
    //先找到第一個頭
    for (; elem; elem = elem?.ts_prev) {
        if (!elem?.ts_prev)
            fireElem = elem
    }

    let tNum
    let tsCheckType = []
    elem = fireElem
    for (; elem; elem = elem?.ts_next) {
        tNum = elem.type
        if (typeof tsCheckType[tNum] == 'undefined') {
            tsCheckType[tNum] = []
            tsCheckType[tNum].push(elem)
        }
        let isFind = tsCheckType[tNum].find((ths) => {
            return isKeysCheck(ths, elem)
        })
        if (!isFind) {
            tsCheckType[tNum].push(elem)
        }
    }
    return tsCheckType
}
function TScompareProDifference(isDiffCheckO) {
    tsCheckType = checkType()
    tsPropDiffs = checkToProType(tsCheckType, voitem, isDiffCheckO)

    let N = tsPropDiffs.constructor.name;
    let mapData = []
    let str = '';
    if (N == 'Array') {

        tsPropDiffs.map((ths, idx) => {
            let json = JSON.stringify(ths, JsonReplacerABC2SVG, 2)
            json = JSON.parse(json)
            json = convertObjecttoString(json)
            if (isDiffCheckO) {
                mapData['mainNumber' + idx] = `let mainNumber${idx}=` + json
                str += mapData['mainNumber' + idx] + '\n';
            }
            else {
                mapData['objNumber' + idx] = `let objNumber${idx}=` + json
                str += mapData['objNumber' + idx] + '\n';
            }

        })
    }
    insertDocumentTextArea(str)
    return mapData
}
function TScompareProInserselect() {
    tsCheckType = checkType()
    tsPropBase = compareProIntersection(tsCheckType)


    let N = tsPropBase.constructor.name;
    let mapData = []
    let str = '';
    if (N == 'Array') {
        mapData = tsPropBase.map((ths, idx) => {
            let json = JSON.stringify(ths, JsonReplacerABC2SVG, 2)
            json = JSON.parse(json)
            json = convertObjecttoString(json)
            return json
        })
        str = mapData.reduce((acc, ths, idx) => {
            acc += `let baseProp${idx}=` + ths + '\n'
            return acc
        }, '')
    }
    insertDocumentTextArea(str)
    return str
}
/**
 * 
 * @param {object | Array} data
 * @param {boolean} isCheckO 是否要跟陣列的第一個比較
 */
/**
 * 
 * @param {any} data
 * @param {any} baseObj  要比對的基礎物件
 * @param {any} isDiffCheckO 是否要跟陣列的第一個比較
 */
function checkToProType(data, baseObj, isDiffCheckO) {
    if (data == null || data === undefined) return '';

    let mapData = []
    let N = data.constructor.name;
    if (N == 'Array') {
        let fireElem = {}
        data.map((ths, idx) => {

            let propDiffs = ths.map((ele, eidx) => {
                if (isDiffCheckO) {
                    if (eidx == 0) {
                        // 跟(base)基礎元素比較，取得屬性不同的地方
                        fireElem = compareProDifference(baseObj, ele)
                        return fireElem
                    } else {
                        // 跟(base)基礎 及第一個元素比較，取得屬性不同的地方
                        let p = compareProDifference(baseObj, ele)
                        return compareProDifference(fireElem, p)
                    }
                } else
                    return compareProDifference(baseObj, ele)
            })

            mapData[idx] = propDiffs
        })
    }
    return mapData
}

/** 比較屬性 的交集不同之處 */
function compareProIntersection(objArr) {
    //compareProDifference
    //取到最簡單的元素屬性

    function getBase(thsArr) {
        let proBase = {}
        let baseKey
        let tagKey
        if (thsArr.constructor.name != 'Array') return
        for (let i = 0; i < thsArr.length; i++) {
            if (i == 0) Object.assign(proBase, thsArr[0])
            baseKey = Object.keys(proBase)
            tagKey = Object.keys(thsArr[i])
            baseKey.forEach((B) => {
                let isFind = tagKey.find((T) => {
                    return B == T
                })
                if (!isFind) {
                    delete proBase[B]
                }
            })
        }
        proBase = compareProDifference(voitem, proBase,)
        return proBase;
    }
    let base = {}

    if (objArr.constructor?.name == 'Array') {
        if (typeof objArr[0].constructor.name != 'Array') {
            base = objArr.map((ths, idx) => {
                return getBase(ths)
            })
        } else {
            base = getBase(objArr)
        }
    }

    //取到最簡單的元素屬性 return (Array[Object] / Object)
    return base
}


/** 比較屬性不同之處 */
function compareProDifference(base, tag) {
    //先 clone 複製
    tag = Object.assign({}, tag)

    let baseKey = Object.keys(base)
    let tagKey = Object.keys(tag)
    baseKey.forEach((B) => {
        let isFind = tagKey.find((T) => {
            return B == T
        })
        //如有找到就 delete 屬性
        if (isFind) {
            delete tag[B]
        }
    })
    return tag
}
/** 比較兩個物件的  keys  是否一樣
 *  淺比較                          */
function isKeysCheck(base, tag) {

    let baseKey = Object.keys(base)
    let tagKey = Object.keys(tag)
    var tagAns = tagKey.every(function (T, index, array) {
        var ans = baseKey.some(function (B, index, array) {
            return B == T
        });
        return ans
    });
    return tagAns
}


/** 轉成 AutoCad 的樂譜 */
function toAcadMusic() {
    let elem = tt123[0].sym.next;
    let fireElem;
    for (; elem; elem = elem?.ts_prev) {
        if (!elem?.ts_prev)
            fireElem = elem
    }
    let nn = fireElem;
    let pos = 0
    let musicAllText = ""
    let m = ["*", "|", "|", "|", "|", "|", "|"]
    let barNum = 0;
    let mm = { jn: "0", oct: "", tempo: "", tone: "", slur: "", choru: "", oneFight: "", towFight: "" }
    let mdot = { jn: ".", oct: "", tempo: "", tone: "", slur: "", choru: "", oneFight: "", towFight: "" }
    let isDot = false
    let musicList = []
    let mete = {}

    var snow = 0;//計數

    ts = []
    //先分軌
    for (; nn; nn = nn?.ts_next) {
        if (!ts[nn.v])
            ts[nn.v] = []
        ts[nn.v].push(nn)

    }
    for (let idx = 0; idx < ts.length; idx++) {
        musicAllText += `<temp_${idx}>\n`
        musicList = []
        m = ["*", "|", "|", "|", "|", "|", "|", "|"]
        barNum = 0;
        //nn = ts[idx]
        for (let x = 0; x < ts[idx].length; x++) {
            nn = ts[idx][x]
            isDot = false
            m=[]
            switch (nn.type) {
                case 6://1: CLEF, 6: METER
                    if ((barNum % 4) != 0)
                        m = ["*", "|", "|", "|", "|", "|", "|", "|"]
                    musicList.forEach((ths) => {
                        m[0] += ths.tone
                        m[1] += ths.oneFight
                        m[2] += ths.oct
                        m[3] += ths.jn
                        m[4] += ths.tempo
                        m[5] += ths.towFight
                        m[6] += ths.slur
                        m[7] += ths.choru
                    })

                    musicList = []
                    musicAllText += m.join("\n")
                    musicAllText += "\n\n"
                    console.log(`king ${snow++}`)

                    musicAllText += `%${barNum+1}\n`;

                    let t = nn.a_meter[0].top
                    let b = nn.a_meter[0].bot
                    mete.top = t
                    mete.bot = b
                    musicAllText += `M:${t}/${b}\n\n`
                    break;
                case 0: //bar
                    barNum++;
                    if ((barNum % 4) == 0 || mete.top>=10) {
                        m = ["*", "|", "|", "|", "|", "|", "|", "|"]
                        musicList.forEach((ths) => {
                            m[0] += ths.tone
                            m[1] += ths.oneFight
                            m[2] += ths.oct
                            m[3] += ths.jn
                            m[4] += ths.tempo
                            m[5] += ths.towFight
                            m[6] += ths.slur
                            m[7] += ths.choru
                        })
                        musicList = []
                        musicAllText += m.join("\n")
                        musicAllText += "\n\n"
                        musicAllText += `%${barNum+1}\n`;
                    }
                    break;
                case 8: // note
                case 10://rest
                    nn.notes.forEach((ths, idx) => {
                        mm = { jn: "0", oct: " ", tempo: " ", tone: " ", slur: " ", choru: " ", oneFight: " ", towFight: " " }
                        mdot = { jn: ".", oct: " ", tempo: " ", tone: " ", slur: " ", choru: " ", oneFight: " ", towFight: " " }
                        //升降記號
                        switch (ths.acc) {
                            case -1: mm.tone = 'b'; break;
                            case 1: mm.tone = '#'; break;
                            case 3: mm.tone = 'o'; break;
                            default: mm.tone = ' '; break;
                        }
                        if (ths.jn == '8')
                            mm.jn = '-'
                        else
                            mm.jn = ths.jn
                        let octType = [-3, -2, -1, 0, 1, 2, 3]
                        let octCode = ["?", ";", ",", " ", ".", ":", "!"]
                        for (i = 0; i < octType.length; i++) {
                            if (ths.jo == octType[i]) {
                                mm.oct = octCode[i]
                                break;
                            }
                        }
                        let tempoList = [
                            { v: 384*1.5,   code: " ", dot: "." },
                            { v: 384,       code: " ", dot: " " },
                            { v: 192*1.5,   code: "-", dot: "." },
                            { v: 192,       code: "-", dot: " " },
                            { v: 128,       code: "3", dot: " " },
                            { v: 96*1.5,    code: "=", dot: "." },
                            { v: 96,        code: "=", dot: " " },
                            { v: 64,        code: "6", dot: " " },
                            { v: 48*1.5,    code: "8", dot: "." },
                            { v: 48,        code: "8", dot: " " },
                            { v: 24*1.5,    code: "g", dot: "." },
                            { v: 24,        code: "g", dot: " " },
                            { v: 12, code: "z", dot: " " }];
                        for (i = 0; i < tempoList.length; i++) {
                            if (nn.dur == tempoList[i].v) {
                                if (tempoList[i].dot == ".") {
                                    mm.tempo = tempoList[i].code
                                    mdot.tempo = tempoList[i].code
                                    isDot = true
                                } else {
                                    isDot = false
                                    mm.tempo = tempoList[i].code
                                }
                                break;

                            }
                        }

                        //let tempoType = [576, 384, 192+96, 192, 128,96+48, 96,48+24, 48,24,12]
                        //let tempoCode = [" ", " ",    "-", "-", "3",  "=","=",  "8","8","g","z"]
                        //let tempoDot = [".", " ",    ".", " ", " ", ".", " ","."," ","",""]
                        //for (i = 0; i < tempoType.length; i++) {
                        //    if (nn.dur == tempoType[i]) {
                        //        if (tempoDot[i] == ".") {
                        //            mm.tempo = tempoCode[i]
                        //            mdot.tempo = tempoCode[i]
                        //            isDot = true
                        //        } else {
                        //            isDot = false
                        //            mm.tempo = tempoCode[i]
                        //        }
                        //        break;
                        //    }
                        //}
                        //指法
                        if (ths.a_dd) {
                            if (ths.a_dd[0].name == "shake") mm.oneFight = "d"
                            if (ths.a_dd[0].name == "////") mm.oneFight = "d"
                            if (ths.a_dd[0].name == "//") mm.oneFight = "d"
                            if (ths.a_dd[0].name == "trill") mm.oneFight = "t"
                            if (ths.a_dd[0].name == ">") mm.oneFight = ">"
                            if (ths.a_dd[0].name == "+") mm.oneFight = "+"
                            if (ths.a_dd[0].name == "segno") mm.oneFight = "s"
                            if (ths.a_dd[0].name == "coda") mm.oneFight = "q"
                        } else if (nn.a_dd) {
                            if (nn.a_dd[0].name == "shake") mm.oneFight = "d"
                            if (nn.a_dd[0].name == "///") mm.oneFight = "d"
                            if (nn.a_dd[0].name == "//") mm.oneFight = "d"
                        }

                        if (nn.slurStart && idx == 0)
                            mm.slur = "("
                        if (nn.slurEnd && idx == 0)
                            mm.slur = ")"

                        if (nn.notes.length > 1) {
                            if (idx == 0) mm.choru = "["
                            if (idx == nn.notes.length - 1)
                                mm.choru = "]"
                        }
                        musicList.push(mm)
                        if (isDot) musicList.push(mdot)
                    })



                    break;
                default:
                    break;
            }

        }
        m = ["*", "|", "|", "|", "|", "|", "|", "|"]
        musicList.forEach((ths) => {
            m[0] += ths.tone
            m[1] += ths.oneFight
            m[2] += ths.oct
            m[3] += ths.jn
            m[4] += ths.tempo
            m[5] += ths.towFight
            m[6] += ths.slur
            m[7] += ths.choru
        })
        musicList=[]
        musicAllText += m.join("\n")
        musicAllText += "\n\n"
    }


    insertDocumentTextArea(musicAllText)
    return musicAllText
}
/** 轉成 AutoCad 的樂譜 */
function toAcadMusicxx() {
    let elem = tt123[0].sym.next;
    let fireElem;
    for (; elem; elem = elem?.ts_prev) {
        if (!elem?.ts_prev)
            fireElem = elem
    }
    let nn = fireElem;
    let pos = 0
    let musicAllText = ""
    let m = ["*", "|", "|", "|", "|", "|", "|"]
    let barAdd = 0;
    let mu = { n: "0", oct: "", tempo: "", tone: "", slur: "", choru: "" }
    let musicList = []

    ts = []
    //先分軌
    for (; nn; nn = nn?.ts_next) {
        if (!ts[nn.v])
            ts[nn.v] = []
        ts[nn.v].push(nn)

    }
    for (let idx = 0; idx < ts.length; idx++) {
        musicAllText += `<temp_${idx}>\n`

        m = ["*", "|", "|", "|", "|", "|", "|"]
        barAdd = 0;
        //nn = ts[idx]
        for (let x = 0; x < ts[idx].length; x++) {
            nn = ts[idx][x]
            switch (nn.type) {
                case 0: //bar
                    if (barAdd >= 8) {
                        musicAllText += m.join("\n")
                        musicAllText += "\n\n"
                        barAdd = 0;
                        m = ["*", "|", "|", "|", "|", "|", "|"]
                    } else
                        barAdd++;
                    break;
                case 8: // note
                case 10://rest
                    //升降記號
                    switch (nn.notes[0].acc) {
                        case -1: m[0] += 'b'; break;
                        case 1: m[0] += '#'; break;
                        default: m[0] += ' '; break;
                    }
                    if (nn.notes[0].jn == '8')
                        m[3] += '-'
                    else
                        m[3] += nn.notes[0].jn

                    switch (nn.notes[0].jo) {
                        case 3: m[2] += '!'; break;
                        case 2: m[2] += ':'; break;
                        case 1: m[2] += '.'; break;
                        case 0: m[2] += ' '; break;
                        case -1: m[2] += ','; break;
                        case -2: m[2] += ';'; break;
                        case -3: m[2] += '?'; break;
                        default: m[2] += ' '; break;
                    }
                    switch (nn.dur) {
                        case 96: m[4] += '='; break;
                        case 128: m[4] += '3'; break;
                        case 192: m[4] += '-'; break;
                        case 288:
                            m[4] += '-'
                            m[0] += ' '
                            m[1] += ' '
                            m[2] += ' '
                            m[3] += '.'
                            m[4] += '-'
                            break;
                        case 384: m[4] += ' '; break;
                        case 576: m[4] += ' ';  //有附點
                            m[0] += ' '
                            m[1] += ' '
                            m[2] += ' '
                            m[3] += '.'
                            m[4] += ' '
                            break;
                        default: m[4] += ' '; break;
                    }


                    break;
                default:
                    break;
            }
        }
        musicAllText += m.join("\n")
        musicAllText += "\n\n"
    }


    insertDocumentTextArea(musicAllText)
    return musicAllText
}
/** 轉成 AutoCad 的 JSON */
function toAcadMusicJSON() {
    let elem = tt123[0].sym.next;
    let fireElem;
    for (; elem; elem = elem?.ts_prev) {
        if (!elem?.ts_prev)
            fireElem = elem
    }
    let nn = fireElem;
    ts = []
    //先分軌
    for (; nn; nn = nn?.ts_next) {
        if (!ts[nn.v])
            ts[nn.v] = []
        ts[nn.v].push(nn)

    }
    let json = JSON.stringify(ts, JsonReplacerABC2SVG, 2)
    insertDocumentTextArea(json)
    return json
}
