namespace MrHihi.HiFileLibrary.Test;

using System.Text;
using MrHihi.HiFileLibrary;
using MrHihi.HiFileLibrary.FileOperation;
using MrHihi.HiFileLibrary.FileOperation.Search;

public class UnitTest1
{
    [Fact]
    public void Test_Traversaling()
    {
        var trav = new DirTraversal();
        trav.Traversaling += Trav_Traversaling;
        trav.FolderRun(".");
    }

    private void Trav_Traversaling(object sender, DirTraversal.TraversalingEventArg e)
    {
    }

    const string SRC = "../../../data/test.txt";
    [Fact]
    public void Test_Search()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var searcher = new FileSearcher()
        {
            SearchTarget = "作品",
            IgnoreCase = true
        };
        
        var result = searcher.Search(SRC);
        Assert.True(result.Count == 2);
    }

    [Fact]
    public void Test_RegexSearch()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var searcher = new FileSearcher()
        {
            SearchTarget = "《[^》]*》",
            IgnoreCase = true
        };
        
        var result = searcher.Search(SRC, new SearchByRegex());
        Assert.True(result.Count == 5);
    }

    [Fact]
    public void Test_Replacer()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        const string SRC_TEST = SRC + ".test";
        File.Copy(SRC, SRC_TEST, true);

        var searcher = new FileSearcher()
        {
            SearchTarget = "作品",
            IgnoreCase = true
        };
        var result = searcher.Search(SRC_TEST);
        var replacer = new FileReplacer(result);

        replacer.Replace("**Replaced**");
        result = searcher.Search(SRC_TEST);
        Assert.True(result.Count == 0);

        var searcher2 = new FileSearcher()
        {
            SearchTarget = "**Replaced**",
            IgnoreCase = true
        };
        var result2 = searcher2.Search(SRC_TEST);
        Assert.True(result2.Count == 2);
    }
}