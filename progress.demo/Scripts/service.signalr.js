+function ($) {
    //使用生成代理
    var connection = $.connection.hub;
    var serviceMonitorHub = $.connection.serviceMonitorHub;
    //设置signalr连接的服务器地址
    connection.url = 'http://127.0.0.1:8000/signalr';

    //Server->Brower
    serviceMonitorHub.client.refresh = function (message) {
        console.log(message);
    }
    //Brower->Server

    //建立连接
    connection.start().done(function (response) {
        console.info("signalr建立成功");
        console.info(response);
    }).fail(function (response) {
        console.log(response);
    });

    // 调用server上的info方法
    $('#btn').on('click', function () {
        serviceMonitorHub.server.info($('#message').val() || "你大爷的我写个日志");
    });
}(jQuery);