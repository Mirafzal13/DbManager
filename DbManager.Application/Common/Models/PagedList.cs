namespace DbManager.Application.Common.Models;

public class PagedList<T>
{
    public PagedList(List<T> data, int total)
    {
        Data = data;
        Total = total;
    }

    public List<T> Data { get; set; }

    public int Total { get; set; }
}