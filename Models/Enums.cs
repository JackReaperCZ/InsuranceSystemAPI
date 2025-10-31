namespace InsuranceSystemAPI.Models
{
    public enum UserRole
    {
        Admin = 1,
        Broker = 2,      // Makler
        Adjuster = 3,    // Likvidátor
        Client = 4       // Klient
    }

    public enum ContractStatus
    {
        Active = 1,      // Aktivní
        Inactive = 2,    // Neaktivní
        Terminated = 3   // Vypovězená
    }

    public enum ClaimStatus
    {
        Reported = 0,       // Nahlášená
        Open = 1,           // Otevřená
        InProgress = 2,     // V řešení
        Processing = 3,     // Ve vyřizování
        Resolved = 4,       // Vyřízená
        Completed = 4,      // Alias pro Vyřízená
        Rejected = 5        // Zamítnutá
    }

    public enum InsuranceType
    {
        Life = 1,           // Životní
        Property = 2,       // Majetková
        Accident = 3,       // Úrazová
        Liability = 4,      // Odpovědnosti
        Travel = 5          // Cestovní
    }

    public enum FileCategory
    {
        Document = 1,       // Dokument
        Photo = 2           // Fotografie
    }
}