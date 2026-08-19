using static ConsoleLog;

public partial class Station {
    // Чтение контактов по TCP
    public void readTCPContact(int addr, int input, bool state) {
        print($"Прочитано состояние контакта {addr}, input {input}: {(state ? "On" : "Off")}", Color.Goldenrod);
        // Обновление обратной связи стрелки
        foreach (TurnPair turnPair in turnPairList) {
            TurnControl tc = turnPair.tc;
            if (tc.fbC.addr == addr && tc.fbC.input == input) {
                turnPair.setFBC(state);
            } else if (tc.fbT.addr == addr && tc.fbT.input == input) {
                turnPair.setFBT(state);
            }
        }
    }

    // Чтение свитчей по TCP
    public void readTCPSwitch(int addr, bool state) {
        print($"Прочитано состояние свитча {addr}: {(state ? "Closed" : "Thrown")}", Color.Goldenrod);

    }

    // Чтение пакетов по TCP
    public void readTCP(TCP_packet_type packet) {
        // print($"readTCP: Header 0x{packet.Header:X2}, Data: {BitConverter.ToString(packet.Data)}", Color.Peru);

        switch(packet.Header) {
            // Status
            case 0x92: { 
                // print($"Сервер вернул статус 0x{packet.Data[2]:X2}"); 
                break; 
            }

            // PING
            case 0x93: { 
                // print($"PING пакет от сервера со статусом 0x{packet.Data[0]:X2}"); 
                break; 
            }

            // Contact
            case 0x94: {
                    if(packet.Data.Length >= 3)
                        readTCPContact(packet.Data[0], packet.Data[1], packet.Data[2] == 1);
                    break; 
                }
            // Switch
            case 0x95: {
                    if (packet.Data.Length >= 3) {
                        byte sw_msb = packet.Data[0];
                        byte sw_lsb = packet.Data[1];
                        ushort sw_addr = (ushort)((sw_msb << 8) | sw_lsb);
                        readTCPSwitch(sw_addr, packet.Data[2] == 1);
                    }
                    break;
                }
            default: break;
        }
    }
}