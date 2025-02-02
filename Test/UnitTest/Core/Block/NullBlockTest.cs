using Core.Block;
using Core.Block.Blocks;
using NUnit.Framework;

namespace Test.UnitTest.Core.Block
{
    public class NullBlockTest
    {
        [Test]
        public void Test()
        {
            var block = new NullBlock();
            Assert.AreEqual(int.MaxValue, block.GetIntId());
            Assert.AreEqual(BlockConst.BlockConst.EmptyBlockId, block.GetBlockId());
        }
    }
}