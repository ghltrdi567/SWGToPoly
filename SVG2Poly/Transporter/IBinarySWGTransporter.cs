using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Transporter
{
	internal interface IBinarySWGTransporter<T> where T : new()
	{
		public void Write(BinaryWriter writer, T source);


		public T Read(BinaryReader reader);



	}
}
