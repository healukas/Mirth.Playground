using System.Text;
namespace Utility;

public static class BinaryUtils
{
    public static string HexDump(this byte[] source, int width=90, int offset = 0, int length = 0)
    {
        var sourceLength = source.Length;
        if (length != 0)
        {
            sourceLength = length;
        }
            
        if (sourceLength + offset > source.Length)
        {
            sourceLength = source.Length - offset; // limit output to at most until the message ends.
        }
        var pre = Convert.ToHexString(source, offset, sourceLength);

        var b = Enumerable.Range(0, pre.Length / 2).Select(i => pre.Substring(i * 2, 2));
        var currentLength = 0;
        var result = "";
        foreach (var x in b)
        {
            if (currentLength >= width)
            {
                currentLength = 0;
                result += "\n";
            }

            result += x;
            result += " ";
            currentLength += 3;
        }
        //result = string.Join(" ", b);

        return result;
    }
    
    public static byte[] Pack(this string message, bool useSize = false, bool addCR = true)
    {
        var totalSize = 2; // start at 2 because of the start byte and the end byte
        var messageBytes = Encoding.ASCII.GetBytes(message);
        var messageSize = messageBytes.Length;
        totalSize += messageSize;
        var messageSizeBytes = Encoding.ASCII.GetBytes(messageSize.ToString("000000"));

        if (useSize) totalSize += 6; // 6 bytes are default for length
        if (addCR) totalSize += 1; // add carriage return at the end

        var framedMessage = new byte[totalSize];
        framedMessage[0] = 0x0B;

        framedMessage[totalSize - 1] =
            0x1C; // set end byte at last position. Will be overwritten, if carriage return is active
        if (addCR)
        {
            framedMessage[totalSize - 2] = 0x1C; // end byte
            framedMessage[totalSize - 1] = 0x0D; // carriage return
        }

        var msgStartIndex = 1;
        if (useSize)
        {
            Array.Copy(messageSizeBytes, 0, framedMessage, 1, 6);
            msgStartIndex = 7; // add 6 bytes for size + 1 start byte (index 0..6)
        }

        Array.Copy(messageBytes, 0, framedMessage, msgStartIndex, messageSize);

        return framedMessage;
    }

    public static string CheckAck(this byte[] binary)
    {
        // <SB>: Start Block character (1 byte). ASCII <VT>, i.e., <0x0B>.
        // This should not be confused with the ASCII characters SOH or STX.
        //     <ACK> or <NAK>: Either the acknowledgement character (1 byte, ASCII <ACK>, i.e., <0x06>) or the negative-acknowledgement character (1 byte, ASCII <NAK>, i.e., <0x15>).
        //     <EB>: End Block character (1 byte). ASCII <FS>, i.e., <0x1C>.
        //     <CR>: Carriage Return (1 byte). ASCII <CR> character, i.e., <0x0D>.

        if (binary.Length < 4)
        {
            return $"ACK/NACK is expected to have a length of 4 bytes. Got {binary.Length} instead.";
        }

        if (binary[0] == 0x0B && binary[1] == 0x06 && binary[2] == 0x1C && binary[3] == 0x0D)
        {
            return "Received ACK";
        }
        
        if (binary[0] == 0x0B && binary[1] == 0x15 && binary[2] == 0x1C && binary[3] == 0x0D)
        {
            return "Received NACK";
        }

        return $"Did not recognize ACK or NACK in message\n{binary.HexDump()}\n";

    }
    
    public static bool Unpack(this byte[] rawMessage, out string message, bool useSize = false, bool useCR = true)
    {
        message = "";
        if (rawMessage[0] != 0x0B || rawMessage[rawMessage.Length - 2] != 0x1C || rawMessage[rawMessage.Length - 1] != 0x0D)
        {
            return false;
        }

        var messageStartIndex = 1;
        var frameLength = 2; // 1 start byte, 1 end bytes
        if (useSize)
        {
            messageStartIndex += 6;
            frameLength += 6;
        }

        if (useCR)
        {
            frameLength += 1; // add a second end byte
        }

        var unframedMessage = new byte[rawMessage.Length - frameLength];
        Array.Copy(rawMessage, messageStartIndex, unframedMessage, 0 ,rawMessage.Length-frameLength);
        
        message = Encoding.ASCII.GetString(unframedMessage).ReplaceLineEndings("\n");
        
        return true;
    }
}