using JAKE.classlibrary.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_library_tests
{
    public class PrototypeTests
    {
        [Fact]
        public void Test_Create_Deep_Copy()
        {
            ZombieMinion minion = new ZombieMinion(1, "green");
            ZombieMinion deepcopy = minion.DeepClone() as ZombieMinion;
            deepcopy.SetId(2);
            deepcopy.SetCurrentPosition(10, 10);

            int originalHash = minion.GetHashCode();
            int copyHash = deepcopy.GetHashCode();

            Assert.NotEqual(copyHash, originalHash);

        }

        [Fact]
        public void Test_Create_Shallow_Copy()
        {
            ZombieMinion minion = new ZombieMinion(1, "green");
            ZombieMinion deepcopy = minion.ShallowClone() as ZombieMinion;

            int originalHash = minion.GetHashCode();
            int copyHash = deepcopy.GetHashCode();

            Assert.Equal(copyHash, originalHash);
        }
    }
}
