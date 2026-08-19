using static ConsoleLog;

public partial class Station {
    // Обновление всех светофоров
    void updateTL(TrafficLight tl) {
        print($"Обновление светофора: {tl.name}, тип: {tl.type}, направление: {tl.dir}");
        // Ищем правило в котором находится светофор
        Rule? rule = atRule(tl);
        // Если правило не найдено - выходим
        if (rule == null) {
            return;
        }
        print($"Светофор {tl.name} находится в правиле: {rule.startBridge?.name} - {rule.finishBridge?.name}");


        // foreach (TrafficLight tl in tlList) {
        //     switch(tl.type) {
        //     // Передвхідний ПАБ
        //     case TrafficLight.Type.PreInPAB:
        //         break;
        //     // Вихідний ПАБ
        //     case TrafficLight.Type.OutPAB:
        //         break;
        //     // Прохідний АБ
        //     case TrafficLight.Type.PassAB:
        //         break;
        //     // Передвхідний АБ
        //     case TrafficLight.Type.PreInAB:
        //         break;
        //     // Вихідний АБ
        //     case TrafficLight.Type.OutAB:
        //         break;
        //     // Вхідний
        //     case TrafficLight.Type.In:
        //         break;
        //     // Вхідний додатковий
        //     case TrafficLight.Type.InAdd:
        //         break;
        //     // Маневровий
        //     case TrafficLight.Type.Shunting:
        //         // Если впереди построен маршрут
                

        //         break;
        //     default:
        //         break;
                
        //     }
            
        // }

        //         // Переключение предыдущего светофора
        // if (prevTL != null) {
        //     switch(prevTL.type) {
        //         // Вихідний ПАБ
        //         case Type.OutPAB:
        //             break;
        //         // Вихідний АБ
        //         case Type.OutAB:
        //             break;
        //         // Вхідний
        //         case Type.In:
        //             // Определяем тип пути (наличие включенных стрелок)
        //             bool isActiveTurn = false;
        //             if (prevTL.dir.turn.Any(t => t.Value)) {
        //                 isActiveTurn = true;
        //             }


        //             // Если текущий - красный, то предыдущий - желтый (прямой путь)
        //             break;
        //         default:
        //             break;
        //     }
        // }

        // // красн -> желтый -> желт.мигающий
        // // желт.мигающий + белый -> желтый
        // // белый -> желтый
        // // зелен -> желт.мигающий
    }
    

}


