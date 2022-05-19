using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IDataAccess
    {
        public Task<SaveState> LoadAsync(string filename);

        public Task SaveAsync(string filename, SaveState state);
    }
}
