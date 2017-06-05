+function ($) {
    var progresstip = $.connection.progressTip;
    console.log(progresstip)
    //更新进度
    progresstip.client.updateProgress = function (progress, timeSpent) {
        $('#progressLab').text(progress);
        $('#timeSpent').text(timeSpent);
    }
    //开始连接
    $.connection.hub.start().done(function (response) {
        console.log(response);
    });
}(jQuery);