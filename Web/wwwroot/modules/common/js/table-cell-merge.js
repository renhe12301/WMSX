function mergeCells(data, fieldName, colspan, target) {
    if (data.length == 0) {
        return;
    }
    var numArr = [];
    var value = data[0][fieldName];
    var num = 0;
    for (var i = 0; i < data.length; i++) {
        if (value != data[i][fieldName]) {
            numArr.push(num);
            value = data[i][fieldName];
            num = 1;
            continue;
        }
        num++;
    }
    if (typeof (value) != "undefined" && value != "") {
        numArr.push(num);
    }
    var merIndex = 0;
    for (var i = 0; i < numArr.length; i++) {
        $(target).bootstrapTable('mergeCells', {
            index: merIndex,
            field: fieldName,
            colspan: colspan,
            rowspan: numArr[i]
        })
        merIndex += numArr[i];
    }
};