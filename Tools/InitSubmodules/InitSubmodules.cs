using LibGit2Sharp;
using System;

namespace InitSubmodules
{
    class InitSubmodules
    {
        static int Main(string[] args)
        {
            try
            {
                InitializeSubmodules();
            }
            catch (Exception ex)
            {
                // never fail the build over this convenience step: when the submodules are already checked out
                // (for example by "git clone --recurse-submodules" or a CI checkout) there is nothing to do anyway
                Console.WriteLine($"warning: InitSubmodules could not check git submodules: {ex.Message}");
                Console.WriteLine("warning: if the build then fails on missing Thirdparty or Interfaces files, run: git submodule update --init --recursive");
            }
            return 0;
        }

        private static void InitializeSubmodules()
        {
            string repoRoot = Repository.Discover(Environment.CurrentDirectory) ??
                throw new System.Exception("InitSubmodules must be run within a git repo.");
            using (Repository repo = new Repository(repoRoot))
            {
                foreach (Submodule submodule in repo.Submodules)
                {
                    SubmoduleStatus submoduleStatus = submodule.RetrieveStatus();
                    bool needsInit = submoduleStatus.HasFlag(SubmoduleStatus.WorkDirUninitialized);
                    if (needsInit)
                    {
                        repo.Submodules.Update(submodule.Name, new SubmoduleUpdateOptions
                        {
                            Init = true
                        });
                    }
                }
            }
        }
    }
}
