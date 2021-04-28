using System;
using System.Collections;
using System.Threading.Tasks;

namespace Code.Domain.Interfaces
{
    public interface IEnemyState
    {
        IEnumerator Execute();
    }
}