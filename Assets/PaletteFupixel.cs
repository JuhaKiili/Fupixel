using System;
using UnityEngine;
using System.Collections;

public class PaletteFupixel : Fupixel {
	[HideInInspector]
	public uint[] indexedPixels;
	
	private Color32[] palette;
	bool paletteDirty = true;
	
	public static Vector3 RGBtoCIELAB( Color color ) {
		// a crude (s)rgb -> xyz -> l*a*b* transform
		Matrix4x4 xyzmat = new Matrix4x4();
		
		xyzmat.SetRow( 0, new Vector4(  0.412453f,  0.357580f,  0.180423f, 0.0f ) );
		xyzmat.SetRow( 1, new Vector4(  0.212671f,  0.715160f,  0.072169f, 0.0f ) );
		xyzmat.SetRow( 2, new Vector4(  0.019334f,  0.119193f,  0.950227f, 0.0f ) );
		xyzmat.SetRow( 3, new Vector4(  0.0f,       0.0f,       0.0f,      1.0f ) );
		
		Vector3 rgb = new Vector3( color.r, color.g, color.b );
		Vector3 xyz = xyzmat.MultiplyPoint3x4( rgb );
		
		float epsilon = 0.008856f;
		Vector3 white = new Vector3( 95.047f, 100.0f, 108.883f );  // D65 white point in XYZ 
		
		float L, a, b;
				
		if( (xyz.y / white.y) > epsilon ) {
			L = 116.0f * Mathf.Pow( (xyz.y / white.y), 1.0f/3.0f ) - 16.0f;
		} else {
			L = 903.3f * xyz.y / white.y;
		}
		
		Vector3 xyz_fact = new Vector3();
		
		if( (xyz.x / white.x) > epsilon ) {
			xyz_fact.x = Mathf.Pow( (xyz.x / white.x), 1.0f/3.0f );
		} else {
			xyz_fact.x = 7.787f * (xyz.x / white.x) + 16.0f / 116.0f;
		}

		if( (xyz.y / white.y) > epsilon ) {
			xyz_fact.y = Mathf.Pow( (xyz.y / white.y), 1.0f/3.0f );
		} else {
			xyz_fact.y = 7.787f * (xyz.y / white.y) + 16.0f / 116.0f;
		}

		if( (xyz.z / white.z) > epsilon ) {
			xyz_fact.z = Mathf.Pow( (xyz.z / white.z), 1.0f/3.0f );
		} else {
			xyz_fact.z = 7.787f * (xyz.z / white.z) + 16.0f / 116.0f;
		}

		
		a = 500.0f * (xyz_fact.x - xyz_fact.y);
		b = 200.0f * (xyz_fact.y - xyz_fact.z);
		
		return new Vector3( L, a, b );
	}
	
	public static uint PaletteFindClosestColor( Color32 color, Color32[] palette ) {	
		Color32 closest;
		uint closest_idx = uint.MaxValue;
		float closest_dist = Mathf.Infinity; 
		
		Vector3 color_lab = RGBtoCIELAB( (Color)color );
		
		for( int i = 0; i < palette.Length; i++ ) {
			Vector3 lab = RGBtoCIELAB( (Color)palette[i] );
			float ds = Vector3.SqrMagnitude( color_lab - lab );
			
			if( ds < closest_dist ) {
				closest = palette[i];
				closest_dist = ds;
				closest_idx = (uint)i;
			}
		}
		
		return closest_idx;
	}
	
	public void BlitTransparent( FURegion region, int x, int y, Color32 transparent ) {
		int w = region.width;
		int h = region.height;
		
		if( x + region.width > this.width - 1 )
			w = (this.width - x);
		
		if( y + region.height > this.height - 1 )
			h = (this.height - y);
		
		for( int i = y; i < y + h; i++ ) {
			for( int j = x; j < x + w; j++ ) {
				Color32 col = region.pixels[ (i-y) * region.width + (j-x) ];
				
				if( col.r != transparent.r || col.g != transparent.g || col.b != transparent.b )
					this.pixels[ i * this.width + j ] = col;
			}
		}
	}
	
	public void BlitWithPalette( FURegion region, int x, int y, Color32[] palette ) {
		int w = region.width;
		int h = region.height;
		
		if( x + region.width > this.width - 1 )
			w = (this.width - x);
		
		if( y + region.height > this.height - 1 )
			h = (this.height - y);
		
		for( int i = y; i < y + h; i++ ) {
			for( int j = x; j < x + w; j++ ) {
				
				if( region.indexedPixels != null ) {
					Color32 col = palette[ region.indexedPixels[ (i-y) * region.width + (j-x) ] ];
					this.pixels[ i * this.width + j ] = col;
				}
				
			}
		}
		
	}
	
	public void BlitWithPalette( FURegion region, int x, int y ) {
		this.BlitWithPalette( region, x, y, this.palette );
	}
	
	public void Blit( FURegion region, int x, int y ) {
		int w = region.width;
		int h = region.height;
		
		if( x + region.width > this.width - 1 )
			w = (this.width - x);
		
		if( y + region.height > this.height - 1 )
			h = (this.height - y);
		
		for( int i = y; i < y + h; i++ ) {
			for( int j = x; j < x + w; j++ ) {
				this.pixels[ i * this.width + j ] = region.pixels[ (i-y) * region.width + (j-x) ];
			}
		}
	}
	
	public FURegion GetRegion( int x, int y, int width, int height ) {
		return new FURegion( this, x, y, width, height );
	}
	
	public FURegion GetScreen() {
		return new FURegion( this, 0, 0, this.width, this.height );
	}
	
	public new void Awake() {
		base.Awake();
		
		this.SetupPalette( 32 );
	}
	
	public void SetupPalette( uint numcols ) {
		this.palette = new Color32[ numcols ];
		
		for( int i = 0; i < numcols; i++ ) {
			this.palette[i] = new Color32( 0, 0, 0, 255 );
		}
		
		this.indexedPixels = new uint[ this.width * this.height ];
		for( int i = 0; i < this.indexedPixels.Length; i++ )
			this.indexedPixels[i] = uint.MaxValue;
		
		this.paletteDirty = false;
	}
	
	public void SetPixel( float x, float y, uint colorIndex ) {
		if( colorIndex < this.palette.Length ) {
			this.indexedPixels[(int)y * width + (int)x] = colorIndex;
			this.SetPixel( x, y, this.palette[colorIndex] );
		}
	}
	
	public void SetPixel( int x, int y, uint colorIndex ) {
		if( colorIndex < this.palette.Length ) {
			this.indexedPixels[y * width + x] = colorIndex;
			this.SetPixel( x, y, this.palette[colorIndex] );
		}
	}
	
	public uint GetPixelPalette( int x, int y ) {
		return this.indexedPixels[ y * width + x ];
	}
	
	public uint GetPixelPalette( int index ) {
		return this.indexedPixels[ index ];
	}
	
	public void ClearPixelsIndexed( uint colorIndex ) {
		if( colorIndex < this.palette.Length ) {
			this.ClearPixels( this.palette[colorIndex] );
			
			for( int i = 0; i < this.indexedPixels.Length; i++ ) {
				this.indexedPixels[i] = colorIndex;
				if( this.indexedPixels[colorIndex] < uint.MaxValue ) 
					this.pixels[i] = this.palette[this.indexedPixels[colorIndex]];
			}
		}		
	}
		
	public void AssignPalette( uint colorIndex, Color32 color ) {
		if( colorIndex < this.palette.Length ) {
			Color32 entry = this.palette[colorIndex];
			
			if( color.r != entry.r || color.g != entry.g || color.b != entry.b || color.a != entry.a ) {
				this.palette[colorIndex] = color;
				this.paletteDirty = true;
			}
		}
	}
	
	public void AssignPalette( uint colorIndex, Color color ) {
		this.AssignPalette( colorIndex, (Color32)color );
	}
	
	public void AssignPalette( uint colorIndex, float r, float g, float b, float a ) {
		this.AssignPalette( colorIndex, new Color( r, g, b, a ) );
	}
	
	public void AssignPalette( uint colorIndex, float r, float g, float b ) {
		this.AssignPalette( colorIndex, new Color( r, g, b, 1.0f ) );
	}
	
	public void AssignPalette( uint colorIndex, byte r, byte g, byte b, byte a ) {
		this.AssignPalette( colorIndex, new Color32( r, g, b, a ) );
	}
	
	public void AssignPalette( uint colorIndex, byte r, byte g, byte b ) {
		this.AssignPalette( colorIndex, new Color32( r, g, b, 255 ) );
	}
	
	public Color32 ReadPalette( uint colorIndex ) {
		if( colorIndex < this.palette.Length ) 
			return this.palette[colorIndex];
		
		return new Color32( 0,0,0,0 );
	}
	
	public Color32[] GetPalette() {
		return this.palette;
	}
	
	public void CommitPalette() {
		if( this.paletteDirty ) {
			for( int i = 0; i < this.indexedPixels.Length; i++ ) {
				if( this.indexedPixels[i] < uint.MaxValue ) {
					this.pixels[i] = this.palette[this.indexedPixels[i]];
				}
			}
			
			this.paletteDirty = false;
		}
	}
	
}
