namespace CardCrawl.Models;

public class Tile
{
    public bool Flipped { get; set; }
    public int AtlasCol;
    public int AtlasRow;
    public int X;
    public int Z;
    public int FullWidth;

    public Tile(bool flipped, int atlasCol, int atlasRow, int x, int z, int fullWidth)
    {
        Flipped = flipped;
        AtlasCol = atlasCol;
        AtlasRow = atlasRow;
        X = x;
        Z = z;
        FullWidth = fullWidth;
    }
}