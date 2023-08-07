package org.healukas;

import ca.uhn.fhir.context.FhirContext;
import ca.uhn.fhir.parser.IParser;
import ca.uhn.hl7v2.DefaultHapiContext;
import ca.uhn.hl7v2.HL7Exception;
import ca.uhn.hl7v2.HapiContext;
import ca.uhn.hl7v2.model.*;
import ca.uhn.hl7v2.model.v25.datatype.CX;
import ca.uhn.hl7v2.model.v25.datatype.XPN;
import ca.uhn.hl7v2.model.v25.message.ADT_A01;
import ca.uhn.hl7v2.model.v25.segment.PID;
import ca.uhn.hl7v2.parser.*;
import org.apache.commons.lang3.NotImplementedException;
import org.hl7.fhir.r4.model.Bundle;
import org.hl7.fhir.r4.model.HumanName;
import org.hl7.fhir.r4.model.Identifier;
import org.hl7.fhir.r4.model.Patient;

public class TransformationTest {
    public TransformationTest(){

    }
    private Bundle Transform_ADT_A01(ADT_A01 adtA01) {
        // create empty FHIR resources
        Bundle result = new Bundle();
        PID pid = adtA01.getPID();
        Patient patient = GeneratePatient(pid);

        Bundle.BundleEntryComponent entry = new Bundle.BundleEntryComponent();
        entry.setRequest(new Bundle.BundleEntryRequestComponent().setMethod(Bundle.HTTPVerb.PUT).setUrl("Patient/" + patient.getId()));
        entry.setResource(patient);
        result.addEntry(entry);
        return result;
    }

    private Patient GeneratePatient(PID pid){
        Patient patient = new Patient();
        // get Identifier for patient

        CX[] identifiers = pid.getPatientIdentifierList();

        for (CX identifier : identifiers) {
            String identifierValue = identifier.getIDNumber().getValue();
            String identifierTypeCode = identifier.getIdentifierTypeCode().getValue();
            if (identifierTypeCode.equals("PI")) {
                patient.setId(UUID5.fromUTF8(identifierValue).toString());
                patient.addIdentifier(new Identifier().setValue(identifierValue).setSystem("https://healex.systems/patient-identifier").setUse(Identifier.IdentifierUse.OFFICIAL));
                break; // let's handle only one identifier for now
            }
        }

        XPN[] names = pid.getPatientName();
        for (XPN name : names) {
            String familyName = name.getFamilyName().getSurname().getValue();
            String givenNames = name.getGivenName().getValue();

            if(familyName.isEmpty() || givenNames.isEmpty()){
                continue;
            }

            String[] givenNameSplit = givenNames.split(" ");
            HumanName fhirPatientName = new HumanName().setFamily(familyName);
            for (String given : givenNameSplit) {
                fhirPatientName.addGiven(given);
            }
            String nameType = name.getNameTypeCode().getValue();

            if (nameType.equals("L")) {
                fhirPatientName.setUse(HumanName.NameUse.OFFICIAL);
            } else if (nameType.equals("M")) {
                fhirPatientName.setUse(HumanName.NameUse.MAIDEN);
            } else {
                fhirPatientName.setUse(HumanName.NameUse.USUAL);
            }

            patient.addName(fhirPatientName);
        }
        return patient;
    }

    private Message Parse(String RawV2Message) {
        ParserConfiguration config = new ParserConfiguration();
        config.setValidating(false); // we do not want validation for now. Just for testing though.
        HapiContext context = new DefaultHapiContext();
        context.setParserConfiguration(config);
        PipeParser ourPipeParser = context.getPipeParser();
        try {
            return ourPipeParser.parse(RawV2Message);
        } catch (HL7Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    public String Transform(String RawMessage) {
        Message parsedMessage = Parse(RawMessage);
        try {
            assert parsedMessage != null;
            Segment segment = (Segment) parsedMessage.get("MSH");
            Bundle result = Dispatch(parsedMessage.getName(), parsedMessage);

            // serialize bundle and return
            FhirContext fhirContext = FhirContext.forR4();
            IParser parser = fhirContext.newJsonParser().setPrettyPrint(true);
            return parser.encodeResourceToString(result);
        } catch (HL7Exception e) {
            throw new RuntimeException(e);
        }
    }

    private Bundle Dispatch(String triggerEvent, Message message) {
        switch (triggerEvent) {
            // handle all the message types here. Maybe we could do this in a dynamic way instead?
            case "ADT_A01" : {
                ADT_A01 adtA01 = (ADT_A01) message;
                //determine A01 or A04
                String realTriggerEvent = adtA01.getMSH().getMessageType().getTriggerEvent().getValue();
                if (realTriggerEvent.equals("A01")) {
                    return Transform_ADT_A01(adtA01);
                } else {
                    throw new NotImplementedException("I don't know what to do with ADT_A04 yet");
                }
            }

            default: throw new NotImplementedException("I can only handle ADT_A01");
        }
    }
}
