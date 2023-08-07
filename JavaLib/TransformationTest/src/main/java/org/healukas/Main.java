package org.healukas;

public class Main {
    public static void main(String[] args) {
        String msg = "MSH|^~\\&|SAP-ISH|001-0001|||20201126172929||ADT^A01|29322403|P|2.5|||AL|NE|DE|ISO8859-15\r" +
                "EVN|A04|20201126172929||NP42I0||20201022143838|BRESGENM\r" +
                "PID|||6847659^9^ISO^^PI||Family^FirstGiven SecondGiven^^^^^L|Maiden|20190101|F|Family||SomeStr. 70^^City^^12324^||||||||||||||||||||\r" +
                "NK1|2|Family^FirstGiven^Second Name^^^^D|AG^Arbeitgeber^MIRTH|Street Address^Other Designation^City^State or Province^A104^DEU^B\r" +
                "PV1||O|00000846^^^00000008|MI||^^|||||||||||||1001665436^0^ISO^^VN||K|||K||||||||||||||||||||20201022143838|||||||V\r" +
                "PV2||^^ISH|||||MI|||||\r";
        System.out.println(new TransformationTest().Transform(msg));
    }


}