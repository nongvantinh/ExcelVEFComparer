using Godot;
using System;
using System.Globalization;
using System.IO;

public class Main : PanelContainer
{
    [Export] private PackedScene _elementScene;
    [Export] private NodePath _contentsHolderPath;
    [Export] private NodePath _addMoreBtnPath;
    [Export] private NodePath _openFileBtnPath;
    [Export] private NodePath _loadDataDialogPath;

    [Export] private NodePath _loadSumPath;
    [Export] private NodePath _deliveredSumPath;
    [Export] private NodePath _load2SumPath;
    [Export] private NodePath _delivered2SumPath;
    [Export] private NodePath _negativeLimitPath;
    [Export] private NodePath _PositiveLimitPath;
    [Export] private NodePath _ratioLoadAndDeliveredPath;
    [Export] private NodePath _vefPath;

    private Node _contentsHolder;
    private TextureButton _addMore;
    private Button _openFile;
    private FileDialog _loadData;
    private Label _loadSum;
    private Label _deliveredSum;
    private Label _load2Sum;
    private Label _delivered2Sum;
    private Label _negativeLimit;
    private Label _positiveLimit;
    private Label _ratioLoadAndDelivered;
    private Label _vef;

    public override void _Ready()
    {
        _contentsHolder = GetNode<Node>(_contentsHolderPath);
        _addMore = GetNode<TextureButton>(_addMoreBtnPath);
        _openFile = GetNode<Button>(_openFileBtnPath);
        _loadData = GetNode<FileDialog>(_loadDataDialogPath);

        _loadSum = GetNode<Label>(_loadSumPath);
        _deliveredSum = GetNode<Label>(_deliveredSumPath);
        _load2Sum = GetNode<Label>(_load2SumPath);
        _delivered2Sum = GetNode<Label>(_delivered2SumPath);
        _negativeLimit = GetNode<Label>(_negativeLimitPath);
        _positiveLimit = GetNode<Label>(_PositiveLimitPath);
        _ratioLoadAndDelivered = GetNode<Label>(_ratioLoadAndDeliveredPath);
        _vef = GetNode<Label>(_vefPath);

        _addMore.Connect("pressed", this, nameof(OnAddmorePressed));
        _openFile.Connect("pressed", this, nameof(OnRequestLoadData));

        _loadData.Connect("file_selected", this, nameof(OnFileSelected));
    }

    private void OnFileSelected(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            while (reader.Peek() != -1)
            {
                string[] separators = { " ", "  ", "   ", "    ", "		", "	", "		" };

                string[] line = reader.ReadLine().Trim().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; line.Length != i; ++i)
                {
                    line[i] = line[i].Remove(line[i].Find(","), 1);
                }

                Element newElement = (Element)_elementScene.Instance();
                _contentsHolder.AddChild(newElement);
                _contentsHolder.MoveChild(_addMore.GetParent(), _contentsHolder.GetChildCount() - 1);
                newElement.Index.Text = (_contentsHolder.GetChildCount() - 1).ToString();

                if (line.Length != 2)
                    continue;
                newElement.TCVShipLoaded.Text = line[0].ToString();
                newElement.TCVShoreDelivered.Text = line[1].ToString();
            }
        }
        UpdateResult();
    }

    private void OnRequestLoadData()
    {
        _loadData.PopupCentered(OS.WindowSize / 2);
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("input_complete"))
        {
            UpdateResult();
        }
    }

    private void OnAddmorePressed()
    {
        Element newElement = (Element)_elementScene.Instance();
        _contentsHolder.AddChild(newElement);
        _contentsHolder.MoveChild(_addMore.GetParent(), _contentsHolder.GetChildCount() - 1);
        newElement.Index.Text = (_contentsHolder.GetChildCount() - 1).ToString();
    }

    private void UpdateResult()
    {
        var elements = _contentsHolder.GetChildren();
        UpdateLoadAndDelivered(elements);

        IsOk(elements);

    }

    private void IsOk(Godot.Collections.Array elements)
    {
        double ratioLoadAndDelivered = RatioOfLoadAndDelivered(elements);
        double negativeLimit = ratioLoadAndDelivered - Math.Round(ratioLoadAndDelivered * 0.003, 5);
        double positiveLimit = ratioLoadAndDelivered + Math.Round(ratioLoadAndDelivered * 0.003, 5);


        // Calculate sum.
        double sumLoad = 0;
        double sumDelivered = 0;

        foreach (var item in elements)
        {
            if (item is Element element)
            {
                double result = 0;
                if (element.ShipShoreRatio.Text != "-")
                    double.TryParse(element.ShipShoreRatio.Text, out result);

                if (negativeLimit <= result && result <= positiveLimit)
                {
                    element.QualVol.Text = "Yes";
                    element.TCVShipLoaded2.Text = double.Parse(element.TCVShipLoaded.Text.ToString()).ToString("N0", new CultureInfo("en-US", false));
                    element.TCVShoreDelivered2.Text =double.Parse(element.TCVShoreDelivered.Text.ToString()).ToString("N0", new CultureInfo("en-US", false));

                    sumLoad += double.Parse(element.TCVShipLoaded.Text.ToString());
                    sumDelivered += double.Parse(element.TCVShoreDelivered.Text.ToString());
                }
                else
                {
                    element.QualVol.Text = "No";
                    element.TCVShipLoaded2.Text = "-";
                    element.TCVShoreDelivered2.Text = "-";
                }
            }
        }

        _ratioLoadAndDelivered.Text = ratioLoadAndDelivered.ToString();
        // Also update sum ok.
        _load2Sum.Text = sumLoad.ToString("N0", new CultureInfo("en-US", false));
        _delivered2Sum.Text = sumDelivered.ToString("N0", new CultureInfo("en-US", false));
        // Also update limit.
        _negativeLimit.Text = negativeLimit.ToString();
        _positiveLimit.Text = positiveLimit.ToString();
        // Update vef.
        _vef.Text = Math.Round(sumLoad / sumDelivered, 4).ToString();
    }

    private double RatioOfLoadAndDelivered(Godot.Collections.Array elements)
    {
        // Calculate sum.
        double sumLoad = 0;
        double sumDelivered = 0;
        foreach (var item in elements)
        {
            if (item is Element element)
            {
                if (element.TCVShipLoaded.Text != "")
                    sumLoad += double.Parse(element.TCVShipLoaded.Text.ToString());
                if (element.TCVShoreDelivered.Text != "")
                    sumDelivered += double.Parse(element.TCVShoreDelivered.Text.ToString());
            }
        }
        // Also update sum.
        _loadSum.Text = sumLoad.ToString("N0");
        _deliveredSum.Text = sumDelivered.ToString("N0");
        return Math.Round(sumLoad / sumDelivered, 5);
    }

    private void UpdateLoadAndDelivered(Godot.Collections.Array elements)
    {
        foreach (var item in elements)
        {
            if (item is Element element)
            {
                FixInputFormating(element);
                UpdateRatio(element);
            }
        }
    }

    private void FixInputFormating(Element element)
    {
        double result;
        if (double.TryParse(element.TCVShipLoaded.Text.Replace(',', '.'), out result))
        {
            element.TCVShipLoaded.Text = result.ToString();
        }
        else
        {
            element.TCVShipLoaded.Text = "";
        }

        if (double.TryParse(element.TCVShoreDelivered.Text.Replace(',', '.'), out result))
        {
            element.TCVShoreDelivered.Text = result.ToString();
        }
        else
        {
            element.TCVShoreDelivered.Text = "";
        }
    }

    private void UpdateRatio(Element element)
    {
        if (element.TCVShipLoaded.Text != "" && element.TCVShoreDelivered.Text != "")
        {
            double load = double.Parse(element.TCVShipLoaded.Text.ToString());
            double delevered = double.Parse(element.TCVShoreDelivered.Text.ToString());
            element.ShipShoreRatio.Text = Math.Round(load / delevered, 5).ToString();
        }
        else
        {
            element.ShipShoreRatio.Text = "-";
        }
    }
}
