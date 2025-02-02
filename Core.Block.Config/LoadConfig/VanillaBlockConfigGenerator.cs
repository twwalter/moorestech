using System.Collections.Generic;
using Core.Block.Config.LoadConfig.ConfigParamGenerator;

namespace Core.Block.Config.LoadConfig
{
    public class VanillaBlockConfigGenerator
    {
        /// <summary>
        ///  各ブロックのコンフィグを生成する
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, IBlockConfigParamGenerator> Generate()
        {
            var config = new Dictionary<string, IBlockConfigParamGenerator>();
            config.Add(VanillaBlockType.Machine, new MachineConfigParamGenerator());
            config.Add(VanillaBlockType.Block, new BlockConfigParamGenerator());
            config.Add(VanillaBlockType.BeltConveyor, new BeltConveyorConfigParamGenerator());
            config.Add(VanillaBlockType.ElectricPole, new ElectricPoleConfigParamGenerator());
            config.Add(VanillaBlockType.Generator, new PowerGeneratorConfigParamGenerator());
            config.Add(VanillaBlockType.Miner, new MinerConfigParamGenerator());

            return config;
        }
    }
}