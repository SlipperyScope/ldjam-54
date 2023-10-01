using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.Andrew;
public class TreeBuilder
{
    private readonly TreeConfig Config;

    public TreeBuilder(TreeConfig config)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public Tree Make()
    {
        var tree = Config.SaplingScene.InstantiateOrNull<Tree>() ?? throw new InvalidCastException(nameof(Config.SaplingScene));
        tree.SetConfig(Config);
        return tree;
    }
}
