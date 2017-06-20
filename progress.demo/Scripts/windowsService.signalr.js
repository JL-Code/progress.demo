+function ($) {
    //使用非生成代理
    var connection = $.hubConnection();
    //设置signalr连接的服务器地址
    connection.url = 'http://127.0.0.1:8000/signalr';
    var serviceMonitorHubProxy = connection.createHubProxy('ServiceMonitorHub');

    //Server->Brower
    serviceMonitorHubProxy.on('refresh', function (response) {
        //console.log(response);
    });
    //建立连接
    connection.start().done(function (response) {
        console.info("signalr建立成功");
        console.info(response);
    }).fail(function (response) {
        console.log(response);
    });

    // 调用server上的info方法
    $('#btn').on('click', function () {
        serviceMonitorHubProxy.invoke('info', $('#message').val());
    });

    $('#stopSite').on('click', function () {
        serviceMonitorHubProxy.invoke('StopIISSite');
    });
    $('#startSite').on('click', function () {
        serviceMonitorHubProxy.invoke('StartIISSite');
    });
}(jQuery);