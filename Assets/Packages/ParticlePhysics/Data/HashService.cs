﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class HashService : System.IDisposable {
		public readonly ComputeBuffer Hashes;

		readonly ComputeShader _compute;
		readonly LifeService _lifes;
		readonly PositionService _positions;
		readonly GridService _grid;
		readonly int _kernelInitHashes;

		public HashService(ComputeShader compute, LifeService l, PositionService p, GridService g) {
			_compute = compute;
			_lifes = l;
			_positions = p;
			_grid = g;
			_kernelInitHashes = compute.FindKernel(ShaderConst.KERNEL_INIT_HASHES);
			Hashes = new ComputeBuffer(l.Lifes.count, Marshal.SizeOf(typeof(int)));
		}
		public void Init() {
			int x = _lifes.SimSizeX, y = _lifes.SimSizeY, z = _lifes.SimSizeZ;
			_grid.SetParams(_compute);
			_compute.SetBuffer(_kernelInitHashes, ShaderConst.BUF_LIFE, _lifes.Lifes);
			_compute.SetBuffer(_kernelInitHashes, ShaderConst.BUF_POSITION, _positions.P0);
			SetBuffer(_compute, _kernelInitHashes);
			_compute.Dispatch(_kernelInitHashes, x, y, z);
		}
		public void SetBuffer(ComputeShader c, int kernel) {
			c.SetBuffer(kernel, ShaderConst.BUF_HASHES, Hashes);
		}

		#region IDisposable implementation
		public void Dispose () {
			if (Hashes != null)
				Hashes.Dispose();
		}
		#endregion		
	}
}