using UnityEngine;
using System.Collections;

public class PaletteFupixel : Fupixel {
	[HideInInspector]
	public uint[] indexedPixels;
	
	private Color32[] palette;
	bool paletteDirty = true;
	
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
