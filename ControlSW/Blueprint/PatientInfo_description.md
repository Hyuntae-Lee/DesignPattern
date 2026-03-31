# PatientInfo Class Specification

1. This module is an independent component.
2. It manages patient information.
3. It has following members.
   * Name    : string // Tag(0010,0010) : Patient's Name
   * ChartNo : string // Tag(0010,0020) : Patient's ID
   * Age     : string // Tag(0010,1010) : Patient's Age
   * Gender  : string // Tag(0010,0040) : Patient's Sex
   * Birthday: string // Tag(0010,0030) : Patient's Birth Date
   * WeightKg: string // Tag(0010,1030) : Patient's Weight (kg)
   * HeightM : string // Tag(0010,1020) : Patient's Size (m)
   * BMI     : string // Tag(0010,1022) : Body Mass Index (kg/m^2)
4. It has following interfaces.
   * LoadPatientInfo(out patient: PATIENT_INFO) : bool
     - Read patient information from the planned location.
   * GetAgeToInt(): int
     - Return the age in integer type
   * GetWeightToDouble(): double
     - Return the weight in double type
   * GetHeightToDouble(): double
     - Return the height in double type
   * UpdateBMI(bUseChildBMI: bool): double
     - Calculate BMI with weight and height so that set it to BMI variable.
   * IsChild(): bool
     - Return true if the age is under 13
   * IsMan(): bool
     - Return true if the gender is 'M' or 'm'.
