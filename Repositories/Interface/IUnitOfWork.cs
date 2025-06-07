namespace AdmissionInfoSystem.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IUniversityRepository Universities { get; }
        IAdmissionNewRepository AdmissionNews { get; }
        IAdmissionMethodRepository AdmissionMethods { get; }
        IAdmissionCriteriaRepository AdmissionCriterias { get; }
        IAcademicProgramRepository AcademicPrograms { get; }
        IMajorRepository Majors { get; }
        IScholarshipRepository Scholarships { get; }
        
        Task SaveChangesAsync();
    }
} 