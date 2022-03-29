
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// µÇÂ¼ÁîÅÆ×é¼þ
    /// </summary>
    public class TokenComponent : Entity,IAwake
    {
        public readonly Dictionary<long,string> TokenDictionary = new Dictionary<long, string>();
    }
}