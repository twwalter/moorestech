using System.Collections.Generic;
using Core.Block.BlockFactory.BlockTemplate;
using Core.Block.Config;
using Core.Block.RecipeConfig;
using Core.Item;
using Core.Ore;

namespace Core.Block.BlockFactory
{
    /// <summary>
    /// バニラのブロックの全てのテンプレートを作るクラス
    /// </summary>
    public class VanillaIBlockTemplates
    {
        public readonly Dictionary<string, IBlockTemplate> BlockTypesDictionary;

        public VanillaIBlockTemplates(
            IMachineRecipeConfig machineRecipeConfig,
            ItemStackFactory itemStackFactory)
        {
            BlockTypesDictionary = new Dictionary<string, IBlockTemplate>();
            BlockTypesDictionary.Add(VanillaBlockType.Machine,
                new VanillaMachineTemplate(machineRecipeConfig, itemStackFactory));
            BlockTypesDictionary.Add(VanillaBlockType.Block, new VanillaDefaultBlock());
            BlockTypesDictionary.Add(VanillaBlockType.BeltConveyor, new VanillaBeltConveyorTemplate(itemStackFactory));
            BlockTypesDictionary.Add(VanillaBlockType.ElectricPole, new VanillaElectricPoleTemplate());
            BlockTypesDictionary.Add(VanillaBlockType.Generator, new VanillaPowerGeneratorTemplate(itemStackFactory));
            BlockTypesDictionary.Add(VanillaBlockType.Miner, new VanillaMinerTemplate(itemStackFactory));
        }
    }
}