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
       /*  this.points[i].z = (oDmFile.GetNamedColumn("Z") - halfOfLengthDH).toFixed(2) > 0 ? (oDmFile.GetNamedColumn("Z") - halfOfLengthDH).toFixed(2) : 0; */
		this.points[i].z = oDmFile.GetNamedColumn("Z").toFixed(2) > 0 ? oDmFile.GetNamedColumn("Z").toFixed(2) : 0;
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


var estTable = function (filePath) {

	var oDmFile = new ActiveXObject("DmFile.DmTableADO");
    oDmFile.Open(filePath, false);

    this.points = [];
    var i = 0;
    for (oDmFile.MoveFirst(); !oDmFile.EOF; oDmFile.MoveNext(), i++) {

        this.points[i] = Object();
        this.points[i].x = oDmFile.GetNamedColumn("XC").toFixed(2) > 0 ? oDmFile.GetNamedColumn("XC").toFixed(2) : 0;
        this.points[i].y = oDmFile.GetNamedColumn("YC").toFixed(2) > 0 ? oDmFile.GetNamedColumn("YC").toFixed(2) : 0;
        this.points[i].z = oDmFile.GetNamedColumn("ZC").toFixed(2) > 0 ? oDmFile.GetNamedColumn("ZC").toFixed(2) : 0;
		this.points[i].au = oDmFile.GetNamedColumn("GC_AU").toFixed(2) > 0 ? oDmFile.GetNamedColumn("GC_AU").toFixed(2) : 0;

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
		oDmFile.SetNamedColumn("CRIT", data[i].criterion);
		oDmFile.SetNamedColumn("AVERAGE", data[i].average);
		switch (data[i].powerZone)
		{
			case 0:
				oDmFile.SetNamedColumn("PZONE0", 1);
				break;
			case 1:
				oDmFile.SetNamedColumn("PZONE1", 1);
				break;
			case 2:
				oDmFile.SetNamedColumn("PZONE2", 1);
				break;
			default:
				oDmFile.SetNamedColumn("PZONE3", 1);
				break;
		}
		
		
    }

    oDmFile.Close();
} 


var setFileValues = function (filePath,key,pairsOfValues) {	
	var oDmFile = new ActiveXObject("DmFile.DmTableADO");
    oDmFile.Open(filePath, false);
	for (oDmFile.MoveFirst(); !oDmFile.EOF; oDmFile.MoveNext(), i++) {
		if( oDmFile.GetNamedColumn(key.field) == key.value )
		{
			for(var i = 0; i < pairsOfValues.length; i++)
			{	
				oDmFile.SetNamedColumn(pairsOfValues[i].field,pairsOfValues[i].value );
			}
		}
	}
	
    oDmFile.Close();
};

var getCountOfRecords = function (filePath) {	
	var oDmFile = new ActiveXObject("DmFile.DmTableADO");
    oDmFile.Open(filePath, false);
	var result=oDmFile.GetRowCount();
    oDmFile.Close();
	return result;
};


var getFileCopyWORecord = function (dhFilePath,exceptedDH) {
	var oDmFileDh = new ActiveXObject("DmFile.DmTableADO");
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	var tempFileName=prjFolder+"\\_dhtemp.dm"
	fso.CopyFile(dhFilePath, tempFileName);
    oDmFileDh.Open(tempFileName, false);
	for (oDmFileDh.MoveFirst(); !oDmFileDh.EOF; oDmFileDh.MoveNext()) {
		if (
					Number(exceptedDH.x).toFixed(2) == oDmFileDh.GetNamedColumn("X").toFixed(2)
				&&	Number(exceptedDH.y).toFixed(2) == oDmFileDh.GetNamedColumn("Y").toFixed(2)
				&&	Number(exceptedDH.z).toFixed(2) == oDmFileDh.GetNamedColumn("Z").toFixed(2)
			)
		{
			oDmFileDh.SetNamedColumn("X", 20);
			oDmFileDh.SetNamedColumn("Y", 20);
			oDmFileDh.SetNamedColumn("Z", 20);
		}
	}
    oDmFileDh.Close();
	return tempFileName;
};

var getDhInsideBm = function(fileNameDH,fileNameBM)
{
	var oDmFileDh = new ActiveXObject("DmFile.DmTableADO");
    oDmFileDh.Open(fileNameDH, false);
	var oDmFileBm = new ActiveXObject("DmFile.DmTableADO");
    oDmFileBm.Open(fileNameBM, false);
	this.points=[];
	var i=0;
	for (oDmFileDh.MoveFirst(); !oDmFileDh.EOF; oDmFileDh.MoveNext()) {
		var xDh = oDmFileDh.GetNamedColumn("X").toFixed(2) > 0 ? oDmFileDh.GetNamedColumn("X").toFixed(2) : 0;
		var yDh = oDmFileDh.GetNamedColumn("Y").toFixed(2) > 0 ? oDmFileDh.GetNamedColumn("Y").toFixed(2) : 0;
		var zDh = oDmFileDh.GetNamedColumn("Z").toFixed(2) > 0 ? oDmFileDh.GetNamedColumn("Z").toFixed(2) : 0;

		for (oDmFileBm.MoveFirst(); !oDmFileBm.EOF; oDmFileBm.MoveNext()) {
			
			var xBm = oDmFileBm.GetNamedColumn("XC").toFixed(2) > 0 ? oDmFileBm.GetNamedColumn("XC").toFixed(2) : 0;
			var yBm = oDmFileBm.GetNamedColumn("YC").toFixed(2) > 0 ? oDmFileBm.GetNamedColumn("YC").toFixed(2) : 0;
			var zBm = oDmFileBm.GetNamedColumn("ZC").toFixed(2) > 0 ? oDmFileBm.GetNamedColumn("ZC").toFixed(2) : 0;
			
			var distance=Math.sqrt(Math.pow(xDh-xBm,2)+Math.pow(yDh-yBm,2)+Math.pow(zDh-zBm,2));
			if(distance<2)
			{
				this.points[i]=Object();
				this.points[i].x = oDmFileDh.GetNamedColumn("X").toFixed(2) > 0 ? oDmFileDh.GetNamedColumn("X").toFixed(2) : 0;
				this.points[i].y = oDmFileDh.GetNamedColumn("Y").toFixed(2) > 0 ? oDmFileDh.GetNamedColumn("Y").toFixed(2) : 0;
				this.points[i].z = oDmFileDh.GetNamedColumn("Z").toFixed(2) > 0 ? oDmFileDh.GetNamedColumn("Z").toFixed(2) : 0;
				this.points[i].fact = oDmFileDh.GetNamedColumn("GC_AU").toFixed(2) > 0 ? oDmFileDh.GetNamedColumn("GC_AU").toFixed(2) : 0;
				this.points[i].estim = 0;
				this.points[i].dist = 0;
				this.points[i].info = oDmFileDh.GetNamedColumn("BHID");
				i = i + 1;
				break;
			}
		}
	}
    oDmFileDh.Close();
	oDmFileBm.Close();
}