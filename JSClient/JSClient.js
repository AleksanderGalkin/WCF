﻿var DHtable = function (filePath, xElPos, yElPos, zElPos) {
    var oDmFile = new ActiveXObject("DmFile.DmTableADO");
    oDmFile.Open(filePath, false);
    this.points = [];
   // this.points = Array(4);
    this.xElPos = xElPos;
    this.yElPos = yElPos;
    this.zElPos = zElPos;
    var i = 0;

    for (oDmFile.MoveFirst(); !oDmFile.EOF; oDmFile.MoveNext(), i++) {
   // for (var j = 0; j < 4; j++, i++) {
        this.points[i] = Object();
        this.points[i].x = oDmFile.GetNamedColumn("X").toFixed(2)>0?oDmFile.GetNamedColumn("X").toFixed(2):0;
        this.points[i].y = oDmFile.GetNamedColumn("Y").toFixed(2)>0?oDmFile.GetNamedColumn("Y").toFixed(2):0;
        this.points[i].z = oDmFile.GetNamedColumn("Z").toFixed(2)>0?oDmFile.GetNamedColumn("Z").toFixed(2):0;
        this.points[i].cr = oDmFile.GetNamedColumn("GC_AU").toFixed(2) > 0 ? oDmFile.GetNamedColumn("GC_AU").toFixed(2) : 0;
        oDmFile.MoveNext();
    }


};