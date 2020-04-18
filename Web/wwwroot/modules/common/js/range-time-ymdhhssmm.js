var rangeTime=function (id,pickEvent) {
    $('#'+id).daterangepicker({
        showDropdowns: true,
        autoUpdateInput:false,
        timePicker:true,
        timePicker24Hour : true,
        timePickerSeconds: true,
        "opens": "center",
        showWeekNumbers: true,
        locale: {
            format: "YYYY-MM-DD HH:mm:ss",
            applyLabel: '确定',
            cancelLabel: '取消',
            daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
            monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                '七月', '八月', '九月', '十月', '十一月', '十二月'],
            firstDay: 1
        },
    }, function(start,end) {

    });

    $('#'+id).on('apply.daterangepicker', function (ev, picker) {
        if(pickEvent)
            pickEvent(picker.startDate.format('YYYY-MM-DD HH:mm:ss'),picker.endDate.format('YYYY-MM-DD HH:mm:ss'));
        $('#'+id).val(picker.startDate.format('YYYY-MM-DD HH:mm:ss')+"至"+picker.endDate.format('YYYY-MM-DD HH:mm:ss'));
    });
    $('#'+id).on('cancel.daterangepicker', function (ev, picker) {
        $('#'+id).val('');
    });
};