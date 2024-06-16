// abc2svg - draw.js - draw functions
//
// Copyright (C) 2014-2023 Jean-Francois Moine
//
// This file is part of abc2svg-core.
//
// abc2svg-core is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// abc2svg-core is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with abc2svg-core.  If not, see <http://www.gnu.org/licenses/>.

// constants
declare interface noteItemxx {
    pit?: number;
    shhd?: number;
    shac?: number;
    dur?: number;
    midi?: number;
    jn?: number;
    jo?: number;
}

declare interface voiceItemxx {
    type?: number;
    v?: number;
    dur?: number;
    time?: number;
    fmt?: FormationInfo,
    p_v?: PageVoiceTune,
    notes?: noteItem[];
    x?: number;
    y?: number;
    wr?: number;
    wl?: number;
    st?: number;
    next?: voiceItem;
    prev?: voiceItem;
    ts_prev?: voiceItem;
    ts_next?: voiceItem;
    err?: boolean;
    nhd?: number;
}
