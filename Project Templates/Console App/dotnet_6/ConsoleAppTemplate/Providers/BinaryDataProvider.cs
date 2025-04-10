namespace ConsoleAppTemplate.Providers;

public class Data
{

}
public interface IDataProvider
{
	Data LoadData();
	void SaveData(Data data);
}

public class BinaryDataProvider : IDataProvider
{
	public Data LoadData()
	{
		throw new System.NotImplementedException();
	}

	public void SaveData(Data data)
	{
		throw new System.NotImplementedException();
	}
}

public class JsonDataProvider : IDataProvider
{
	public Data LoadData()
	{
		throw new System.NotImplementedException();
	}

	public void SaveData(Data data)
	{
		throw new System.NotImplementedException();
	}
}