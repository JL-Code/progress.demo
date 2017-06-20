
//module wxrbt.manager {
//    export const enum ServiceStatus {
//        /**服务停止*/
//        Stopped = 1,
//        /**正在运行*/
//        StartPending = 2,
//        /**正在停止*/
//        StopPending = 3,
//        /**运行中*/
//        Running = 4,
//        /**正在继续*/
//        ContinuePending = 5,
//        /**正在暂停*/
//        PausePending = 6,
//        /**已暂停*/
//        Paused = 7,
//    }
//    interface IService {
//        DisplayName: string;
//        ServiceName: string;
//        Status: ServiceStatus
//    }
//    /**管理服务*/
//    export class service {
//        private proxy: SignalR.Hub.Proxy;
//        private $: JQueryStatic;
//        private ip: string;
//        private port: number;
//        constructor(ip: string, port: number) {
//            this.ip = ip;
//            this.port = port;
//        }
//        /**
//         * 开启服务运行状态监测
//         * @param {(services} callback
//         */
//        start(callback: (services: Array<IService>) => void) {
//            jQuery.getScript("http://" + this.ip + ":" + this.port + "/signalr/hubs", () => {
//                jQuery.connection.hub.url = "http://" + this.ip + ":" + this.port + "/signalr";
//                this.proxy = jQuery.signalR.hub.createHubProxy("ServiceMonitorHub");

//                //每次刷新数据回调
//                this.proxy.on("refresh", (services: Array<IService>) => {
//                    callback(services);
//                });


//                jQuery.connection.hub.start().fail(() => {
//                    alert("连接实时消息服务期：http://" + this.ip + ":" + this.port + "失败，请确认消息服务配置正确且正常开启！");
//                });
//            });
//        }
//    }
//}