+function ($) {
    //使用非生成代理
    var connection = $.hubConnection();
    //设置signalr连接的服务器地址
    connection.url = 'http://127.0.0.1:8000/signalr';
    var serviceMonitorHubProxy = connection.createHubProxy('ServiceMonitorHub');

    //Server->Brower
    serviceMonitorHubProxy.on('refresh', function (response) {
        console.log(response);
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
        var message = $('#message').val();
        console.log(message);
        serviceMonitorHubProxy.invoke('info', message);
    });

    $('#stopSite').on('click', function () {
        var message = $('#message').val();
        console.log(message);
        serviceMonitorHubProxy.invoke('StopIISSite', message);
    });
    $('#startSite').on('click', function () {
        var message = $('#message').val();
        console.log(message);
        serviceMonitorHubProxy.invoke('StartIISSite', message);
    });

    $('#siteUpgrade').on('click', function () {
        var message =
            {
                ZipFilePath: "E:\\PublishOutput.zip",
                DecompressionPath: "E:\\PublishOutput",
                CopyPath: "E:\\PublishOutput\\*",
                MainSite:
                {
                    Name: "PowerShellSite",
                    PhysicalPath: "E:\\03_ReleaseWebSite\\PowerShellSite",
                    DomainName: "http://localhost:805/home/windowsSignalrTest",
                    Port: 805
                }
            }
        console.log(JSON.stringify(message));
        alert(message.MainSite.PhysicalPath);
        serviceMonitorHubProxy.invoke('UpgradeSite', message);
    });
}(jQuery);