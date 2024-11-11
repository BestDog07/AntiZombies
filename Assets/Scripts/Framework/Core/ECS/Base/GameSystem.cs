using System.Collections.Generic;
using Framework.Core.ECS.Base;

namespace Runtime.ECS
{
    public class GameSystem : Framework.Core.ECS.Base.System
    {
        public List<Framework.Core.ECS.Base.System> systems = new List<Framework.Core.ECS.Base.System>();
        public override void Create(Context context)
        {
            throw new System.NotImplementedException();
        }

        public override void Init(Context context)
        {
            throw new System.NotImplementedException();
        }

        public override void Update(Context context)
        {
            throw new System.NotImplementedException();
        }

        public override void Destroy(Context context)
        {
            throw new System.NotImplementedException();
        }
    }
}