/// binaryファイルかどうか判定するプログラム
/// [ref]
/// ※18年前の情報
/// https://rarara.org/community/programming/%E3%83%86%E3%82%AD%E3%82%B9%E3%83%88%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%81%A8%E3%83%90%E3%82%A4%E3%83%8A%E3%83%AA%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%82%92%E5%88%A4%E5%88%A5%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95%E3%81%AF%EF%BC%9F/


/* Output
sample.7z is binary
sample.arc is binary
sample.bat is not binary
sample.bmp is binary
sample.docx is binary
sample.exe is binary
sample.gif is binary
sample.jpg is binary
sample.mp3 is binary
sample.pea is binary
sample.png is binary
sample.pptx is binary
sample.rtf is not binary
sample.tar is binary
sample.tif is binary
sample.txt is not binary
sample.txt.bcm is not binary
sample.txt.br is binary
sample.txt.bz2 is binary
sample.txt.gz is binary
sample.xlsx is binary
sample.zip is binary
sample.zpaq is binary
 */
foreach (string file in Directory.EnumerateFiles("../../../resources"))
{
    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
    {
        bool isZeroByte = true;
        for (;;)
        {
            bool isBinary = false;
            byte[] bs = new byte[0x1000];
            int readSize = fs.Read(bs, 0, bs.Length);
            Array.Resize(ref bs, readSize);
            if (readSize == 0)
            {
                // 0バイトのファイルは判定不可
                // フォーマット情報がファイルから得られないのでファイル名から判断する必要あり
                // 例えば、Windowsで新しいファイルの作成からテキストファイルやパワーポイントファイルを作った場合、共に0byteで差異はない
                if (isZeroByte)
                {
                    Console.WriteLine(Path.GetFileName(file) + " is emptyfile");
                    break;
                }
                else
                {
                    Console.WriteLine(Path.GetFileName(file) + " is not binary");
                    break;
                }
            }
            isZeroByte = false;

            foreach (var b in bs)
            {
                if (b == 0x00)
                {
                    isBinary = true;
                    break;
                }
            }
            if (isBinary)
            {
                Console.WriteLine(Path.GetFileName(file) + " is binary");
                break;
            }
        }
    }
}
Console.ReadKey();

