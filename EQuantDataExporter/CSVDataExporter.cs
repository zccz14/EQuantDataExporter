using EQuant;
using EQuant.STG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[assembly: Strategy(typeof(EQuantDataExporter.CSVDataExporter))]
namespace EQuantDataExporter {
	public class CSVDataExporter : FixedStrategyDefine {
		public CSVDataExporter () : base(new Guid("{C89EB9A6-454E-49D9-ACD0-66E2B01CE040}")) {
			Contracts[0].ShiftCount = 1;
			Subgroup = "tools";
			Periods.Add(MinuteType.BarMin1);
			Periods.Add(MinuteType.BarMin2);
			Periods.Add(MinuteType.BarMin3);
			Periods.Add(MinuteType.BarMin5);
			Periods.Add(MinuteType.BarMin10);
			Periods.Add(MinuteType.BarMin15);
			Periods.Add(MinuteType.BarMin20);
			Periods.Add(MinuteType.BarMin30);
			Periods.Add(MinuteType.BarHour1);
			Periods.Add(MinuteType.BarHour2);
			Periods.Add(MinuteType.BarDay);

			StringParameterDefine path = new StringParameterDefine("path");
			path.Title = "输出文件路径";
			ParameterDefines.Add(path);
		}
		public override int ContractCount => 1;

		public override string Name => "CSV Data Exporter (Bar)";
		public override string Company => "zccz14";
		public override string Title => "CSV Data Exporter (Bar) v1.0.0";
		public override string Description => "Export original price data set as a csv file";

		protected override StrategyExecuter CreateExecuter () {
			return new Executer();
		}

		private class Executer : StrategyExecuter {
			private struct Param {
				public string Path;
			}
			private Param _arg;
			protected override void OnProgressStarted () {
				Contracts[0].OnBar += Executer_OnBar;
			}

			private void Executer_OnBar (object sender, PushBarEventArgs e) {
				_barList.Add(e.Bar);
			}

			private readonly List<IBar> _barList = new List<IBar>();

			protected override void OnSetConfig (StrategyConfig config) {
				_arg.Path = config["path"].StringValue;
			}
			protected override void OnProgressEnded () {
				using (StreamWriter sw = new StreamWriter(_arg.Path)) {
					int size = _barList.Count;
					sw.WriteLine("datetime,open,high,low,close,volume,open_interest");
					for (int i = 0; i < size; i++) {
						IBar bar = _barList[i];
						object[] data = {
							bar.DateTime,
							bar.OpenPrice,
							bar.MaxPrice,
							bar.MinPrice,
							bar.ClosePrice,
							bar.Volume,
							bar.OpenInterest
						};
						sw.WriteLine(data.Aggregate((pre, cur) => pre + "," + cur));
					}
				}
			}

			protected override void OnDispose()
			{
				_barList.Clear();
			}
		}
	}
}
