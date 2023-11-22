namespace Service.ViewModels
{
    public partial class MessageViewModel
    {
        public int MessageId { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string? Content { get; set; }
        public DateTime? SentTime { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsArchived { get; set; }
    }
}
