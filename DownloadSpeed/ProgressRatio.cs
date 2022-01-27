using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadSpeed
{
    public class ProgressRatio
    {
        double progress_ratio = 1.0;

        // 构造 ProgressRatio
        // parameters:
        //      length  进度条最大范围 long 类型值
        public ProgressRatio(long length)
        {
            this.progress_ratio = (double)64000 / (double)length;
            if (this.progress_ratio > 1.0)
                this.progress_ratio = 1.0;
        }

        // 把 long 类型值转换为 int 类型
        public int GetInteger(long value)
        {
            return (int)(this.progress_ratio * (double)value);
        }
    }
}
