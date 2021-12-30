using System.Collections.Generic;
using Core.Block.Config.LoadConfig.Param;

namespace Core.Block.Config.LoadConfig.ConfigParamGenerator
{
    public class PowerGeneratorConfigParamGenerator : IBlockConfigParamGenerator
    {
        public BlockConfigParamBase Generate(dynamic blockParam)
        {
            var fuelSettings = new List<FuelSetting>();
            foreach (var fuel in blockParam.fuel)
            {
                var id = fuel.id;
                var time = fuel.time;
                var power = fuel.power;
                fuelSettings.Add(new FuelSetting(id,time,power));
            }
            
            return new PowerGeneratorConfigParam(fuelSettings);
        }
    }
}