using Plugin.Maui.Audio;
using System.Net;
using System.Text.Json;

namespace MauiApp2;

public partial class MainPage : ContentPage
{
    private readonly IAudioManager audioManager;

    public MainPage(IAudioManager audioManager)
    {
        InitializeComponent();
        this.audioManager = audioManager;
    }
    void OnEntryCompleted(object sender, EventArgs e)
    {
        OnCounterClicked(sender,e);
    }
    private void OnCounterClicked(object sender, EventArgs e)
    {
        String wordLink = $"https://cdn.jsdelivr.net/gh/lyc8503/baicizhan-word-meaning-API/data/words/{wordEntry.Text}.json";
        try
        {
            wordLabel.Text = toReadJson(getWordInfo(wordLink));
        }
        catch (System.Net.WebException e1)
        {

            DisplayAlert("错误", "网络错误或单词错误，请检查网页链接或检查单词拼写（也可能这词api里没得）", "OK");
        }
        SemanticScreenReader.Announce(CounterBtn.Text);
    }
    private String getWordInfo(String wordLink)
    {
        WebRequest req = WebRequest.Create(wordLink); //创建webrequest链接
        HttpWebResponse response = (HttpWebResponse)req.GetResponse();
        Stream strean = response.GetResponseStream(); //转为流
        StreamReader sr = new StreamReader(strean);
        return sr.ReadToEnd();
    }
    private String toReadJson(String jsonStr)
    {
        toolClass? toolclass = JsonSerializer.Deserialize<toolClass>(jsonStr);//反序列化json
        return $"单词:{toolclass?.word}\n音标:{toolclass?.accent}\n中文释义:{toolclass?.mean_cn}\n英文释义:{toolclass?.mean_en}\n例子:{toolclass?.sentence}\n例子翻译:{toolclass.sentence_trans}";
    }

    private void SoundBtn_Clicked(object sender, EventArgs e)
    {
        playsound();
    }

    private async void playsound()
    {
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("dictvoice.mp3"));
        player.Play();
        string str1 = System.Environment.CurrentDirectory;
        File.Delete(str1+"\Raw\dictvoice.mp3");
    }
}
public class toolClass
{
    public string? word { get; set; }
    public string? mean_cn { get; set; }
    public string? mean_en { get; set; }
    public string? sentence { get; set; }
    public string? sentence_trans { get; set; }
    public string? accent { get; set; }
}

