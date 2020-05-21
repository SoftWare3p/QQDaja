using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua.Plugins.Parrot.MahuaService;
namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 插件初始化事件
    /// </summary>
    public class InitializationMahuaEvent
        : IInitializationMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly ITickServices _tickServices;
        public InitializationMahuaEvent(
            IMahuaApi mahuaApi, ITickServices tickServices)
        {
            _mahuaApi = mahuaApi;
            _tickServices = tickServices;
        }
        public void Initialized(InitializedContext context)
        {
            _tickServices.StartAsync().GetAwaiter().GetResult();         
        }
        
    }
}
