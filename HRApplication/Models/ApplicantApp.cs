namespace HRApplication.Models
{
    public class ApplicantApp
    {
        public string? Name { get; set; }
        public string? address { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? gender { get; set; }
        public DateTime birth_date { get; set; }
        public string? nationality { get; set; }
        public string? Military { get; set; }
        public string? Marital { get; set; }
        public string? appliedPosition { get; set; }
        public string? arabicSpoken { get; set; }
        public string? arabicWritten { get; set; }
        public string? arabicUnderstanding { get; set; }
        public string? englishSpoken { get; set; }
        public string? englishWritten { get; set; }
        public string? englishUnderstanding { get; set; }
        public List<Experience>? experiences { get; set; }
        public List<Languages>? Languages { get; set; }
        public string? highgSchoolName { get; set; }
        public string? highgSchoolDegree { get; set; }
        public DateTime highgSchoolGraduationDate { get; set; }
        public string? highgSchoolMajor { get; set; }
        public string? universityName { get; set; }
        public string? universityDegree { get; set; }
        public DateTime universityGraduationDate { get; set; }
        public string? universityMajor { get; set; }
        public string? currentlyEmployed { get; set; }
        public string? relatives { get; set; }
        public string? applyAtRaya { get; set; }
        public string? additionalTraining { get; set; }
        public string? computerSkills { get; set; }

    }
}
