var DHtable = function (filePath) {
    var oDmFile = new ActiveXObject("DmFile.DmTableADO");
    oDmFile.Open(filePath, false);
    this.points = [];
    var i = 0;
    for (oDmFile.MoveFirst(); !oDmFile.EOF; oDmFile.MoveNext(), i++) {
        var halfOfLengthDH = oDmFile.GetNamedColumn("LENGTH").toFixed(2) / 2;
        this.points[i] = Object();
        this.points[i].x = oDmFile.GetNamedColumn("X").toFixed(2) > 0 ? oDmFile.GetNamedColumn("X").toFixed(2) : 0;
        this.points[i].y = oDmFile.GetNamedColumn("Y").toFixed(2) > 0 ? oDmFile.GetNamedColumn("Y").toFixed(2) : 0;
        this.points[i].z = (oDmFile.GetNamedColumn("Z") - halfOfLengthDH).toFixed(2) > 0 ? (oDmFile.GetNamedColumn("Z") - halfOfLengthDH).toFixed(2) : 0;
        this.points[i].cr = oDmFile.GetNamedColumn("GC_AU").toFixed(2) > 0 ? oDmFile.GetNamedColumn("GC_AU").toFixed(2) : 0;
        this.points[i].info = oDmFile.GetNamedColumn("BHID");

    }
    oDmFile.Close();

};

var BMtable = function (filePath, xAxis, yAxis, zAxis, xElPos, yElPos, zElPos) {
    var oDmFile = new ActiveXObject("DmFile.DmTableADO");
    oDmFile.Open(filePath, false);

    this.points = [];
    this.xAxis = yAxis;  //оси x и y меняем местами, так как расчет эллипса происходит относительно оси x
    this.yAxis = xAxis;  // в расчете x это полуось a, y - полуось b
    this.zAxis = zAxis;  //  затем полуось a разворачивается по азимуту (y) так как угол поворота отсчитывается от азимута
    this.xElPos = yElPos;
    this.yElPos = xElPos;
    this.zElPos = zElPos;
    var i = 0;
    for (oDmFile.MoveFirst(); !oDmFile.EOF; oDmFile.MoveNext(), i++) {

        this.points[i] = Object();
        this.points[i].x = oDmFile.GetNamedColumn("XC").toFixed(2) > 0 ? oDmFile.GetNamedColumn("XC").toFixed(2) : 0;
        this.points[i].y = oDmFile.GetNamedColumn("YC").toFixed(2) > 0 ? oDmFile.GetNamedColumn("YC").toFixed(2) : 0;
        this.points[i].z = oDmFile.GetNamedColumn("ZC").toFixed(2) > 0 ? oDmFile.GetNamedColumn("ZC").toFixed(2) : 0;

    }
    oDmFile.Close();

};

var writeResultToFile = function (filePath, data) {
    var oDmFile = new ActiveXObject("DmFile.DmTableADO");
    oDmFile.Open(filePath, false);
    var i = 0;
    for (oDmFile.MoveFirst(); !oDmFile.EOF; oDmFile.MoveNext(), i++) {

        oDmFile.SetNamedColumn("TRDIPDIR", data[i].trDipDir);
        oDmFile.SetNamedColumn("TRDIP", data[i].trDip);
    }

    oDmFile.Close();
}