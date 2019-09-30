use std::fmt::{Display, Formatter};
use std::net::SocketAddr;

pub struct PlayerPackage
{
    unique_key: String,

    x_pos: f32,
    y_pos: f32,

    z_rot: f32
}

impl PlayerPackage
{
    pub fn get_key(&self) -> String { self.unique_key.clone() }
    ///
    /// Converts a string slice into a PlayerPackage. The data
    /// 
    /// #data
    /// 
    /// The string slice which represents the data - converted into
    /// position and rotation
    /// 
    /// #Return
    /// 
    /// Returns an Ok(PlayerPackage) if all data members were
    /// successfully parsed - Err(String) with message otherwise
    pub fn from_str(data: &str) -> Result<PlayerPackage, String>
    {
        let items = data.split(',').collect::<Vec<&str>>();
        if items.len() == 4
        {
            let x_pos = items[1].parse::<f32>();
            let y_pos = items[2].parse::<f32>();
            let z_rot = items[3].parse::<f32>();

            if x_pos.is_ok() && y_pos.is_ok() && z_rot.is_ok()
            {
                return Ok(
                    PlayerPackage
                    {
                        unique_key: items[0].to_string(),
                        x_pos: x_pos.unwrap(),
                        y_pos: y_pos.unwrap(),
                        z_rot: z_rot.unwrap()
                    }
                )
            }
        }
        Err(format!("Could not parse data: {}", data))
    }

    ///
    /// Converts all data into a formatted String, which can be sent to
    /// clients
    /// 
    pub fn to_data_string(&self) -> String
    {
        format!("{},{},{},{}", self.unique_key, self.x_pos, self.y_pos, self.z_rot)
    }
}

///
/// The implementation for PlayerPackage's Display trait, which
/// allows it to be console printed
///
impl Display for PlayerPackage
{
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result
    {
        write!(f, "MacAddr: {}, Pos: ({}, {}), ZRot: {}", self.unique_key, self.x_pos, self.y_pos, self.z_rot)
    }
}

