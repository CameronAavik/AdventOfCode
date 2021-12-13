namespace AdventOfCode.CSharp.Common;

public static class OCR
{
    // borrowing OCR table from https://github.com/willkill07/AdventOfCode2016/blob/master/src/Day08.cpp
    public static char MaskToLetter(int letterPixels) => letterPixels switch
    {
        0x19297A52 => 'A',
        0x392E4A5C => 'B',
        0x1928424C => 'C',
        0x39294A5C => 'D',
        0x3D0E421E => 'E',
        0x3D0E4210 => 'F',
        0x19285A4E => 'G',
        0x252F4A52 => 'H',
        0x1C42108E => 'I',
        0x0C210A4C => 'J',
        0x254C5292 => 'K',
        0x2108421E => 'L',
        0x23BAC631 => 'M',
        0x252D5A52 => 'N',
        0x19294A4C => 'O',
        0x39297210 => 'P',
        0x19295A4D => 'Q',
        0x39297292 => 'R',
        0x1D08305C => 'S',
        0x1C421084 => 'T',
        0x25294A4C => 'U',
        0x2318A944 => 'V',
        0x231AD6AA => 'W',
        0x22A22951 => 'X',
        0x23151084 => 'Y',
        0x3C22221E => 'Z',
        0x00000000 => ' ',
        _ => '?'
    };
}
