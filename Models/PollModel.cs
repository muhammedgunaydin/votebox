namespace vote_box.Models
{
    public class PollModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ICollection<OptionModel> Options { get; set; } = new List<OptionModel>();
        public ICollection<VoteModel> Votes { get; set; }
    }
}