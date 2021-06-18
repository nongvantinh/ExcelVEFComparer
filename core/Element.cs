using Godot;
using System;

public class Element : PanelContainer
{
    [Export] private NodePath _indexPath;
    [Export] private NodePath _tcvShipLoadedPath;
    [Export] private NodePath _tcvShoreDelivered;
    [Export] private NodePath _shipShoreRatio;
    [Export] private NodePath _qualVol;
    [Export] private NodePath _tcvShipLoaded2Path;
    [Export] private NodePath _tcvShoreDelivered2Path;

    public Label Index;
    public LineEdit TCVShipLoaded;
    public LineEdit TCVShoreDelivered;
    public Label ShipShoreRatio;
    public Label QualVol;
    public Label TCVShipLoaded2;
    public Label TCVShoreDelivered2;

    public override void _Ready()
    {
        Index = GetNode<Label>(_indexPath);
        TCVShipLoaded = GetNode<LineEdit>(_tcvShipLoadedPath);
        TCVShoreDelivered = GetNode<LineEdit>(_tcvShoreDelivered);
        ShipShoreRatio = GetNode<Label>(_shipShoreRatio);
        QualVol = GetNode<Label>(_qualVol);
        TCVShipLoaded2 = GetNode<Label>(_tcvShipLoaded2Path);
        TCVShoreDelivered2 = GetNode<Label>(_tcvShoreDelivered2Path);
    }
}
